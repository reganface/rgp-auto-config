using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RGP_Auto_Config
{
    public partial class Form1 : Form
    {
        private bool altBtn = false;
        private List<List<string>> multipleRemotes = new List<List<string>>();
        private string masterKeyEncrypted;

        public Form1()
        {
            InitializeComponent();

            // if image file exists, use that
            if (File.Exists("logo.png"))
            {
                this.logo.ImageLocation = @"logo.png";
            } else
            {
                this.logo.Image = global::RGP_Auto_Config.Properties.Resources.mesarim_logo;
            }

            //this.logo.Show();
            
        }

        // user is requesting to update RGP connection
        private void signInBtn_Click(object sender, EventArgs e)
        {
            // check for alternate btn function
            // this is for selecting from multiple locations
            if (this.altBtn)
            {
                if (locationSelect.SelectedValue == null)
                {
                    return;
                }

                string[][] remotes = this.multipleRemotes.Select(a => a.ToArray()).ToArray();
                string[] newRemote = remotes[Convert.ToInt32(locationSelect.SelectedValue)];
                updateStatus("Updating config");
                updateRegistry(newRemote, this.masterKeyEncrypted);
                return;
            }

            string pin = this.inputPin.Text;
            string msg;

            // check for running RGP
            Process[] pname = Process.GetProcessesByName("rockgympro");
            if (pname.Length != 0)
            {
                msg = "Rock Gym Pro is currently running.  Close all Rock Gym Pro windows and try again.\r\n\r\n";
                msg += "If you don't see RGP running, it may have a task going in the background.  Try again in a minute or two as it should be done by then.";
                MessageBox.Show(msg, "Close RGP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // only make call if pin is not empty
            if (pin.Length > 0)
            {

                // get existing connection info
                updateStatus("Getting current connection");
                string[] info = getConnectionInfo();
                if (info == null)
                {
                    updateStatus("Enter your PIN to configure RGP");
                    msg = "Can't find connection info for RGP.";
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // get info for all remote connections
                updateStatus("Looking up connected facilites");
                string[,] remoteInfo = getRemoteInfo(info, pin);

                if (remoteInfo == null)
                {
                    updateStatus("Enter your PIN to configure RGP");
                    return;
                }

                updateStatus("Selecting local facility");
                string localIP = getLocalIP(info[0]);
                string subnetMask = GetSubnetMask(localIP);
                string[][] remotes = findNewRemote(remoteInfo, localIP, subnetMask, info[0]);
                
                if (remotes == null)
                {
                    updateStatus("Enter your PIN to configure RGP");
                    return;
                }

                if (remotes.Length == 1)
                {
                    // single match, update the connection in registry
                    string[] newRemote = remotes[0];
                    updateStatus("Updating config");
                    updateRegistry(newRemote, info[4]);                    
                }
                else
                {
                    // multiple connections available
                    updateStatus("Multiple RGP servers exist on this subnet.  Choose your location");
                    locationSelect.DisplayMember = "Text";
                    locationSelect.ValueMember = "Value";
                    List<Object> items = new List<Object>();

                    
                    for (int i = 0; i < remotes.Length; i++)
                    {
                        items.Add(new { Text = remotes[i][4], Value = i }); // add each item and index to combo box
                        this.multipleRemotes.Add(new List<string>(remotes[i]));
                    }

                    locationSelect.DataSource = items;
                    inputPin.Hide();        // hide pin input
                    locationSelect.Show();  // show location select combobox
                    this.altBtn = true;     // flag for when the ok button is pressed again
                    this.masterKeyEncrypted = info[4];


                }
                
                
            }
            else
            {
                updateStatus("Missing PIN");
            }                      
        }


        


        // get RGP registry data of current connection
        public string[] getConnectionInfo()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VB and VBA Program Settings\RockGymPRO\Config"))
            {
                IPAddress o;
                IPHostEntry hostEntry;

                if (key != null)
                {
                    string[] data = new string[5];
                    data[0] = key.GetValue("dbserver").ToString();   // dbserver
                    data[1] = key.GetValue("dbname").ToString();   // dbname
                    data[2] = key.GetValue("dbpass").ToString();   // dbpass
                    data[3] = key.GetValue("dbuser").ToString();   // dbuser
                    data[4] = key.GetValue("MasterEncryptionKey").ToString();   // master key

                    // check to make sure all values are there
                    for (int i = 0; i < 5; i++)
                    {
                        if (string.IsNullOrEmpty(data[i]))
                        {
                            string msg = "Could not find connection info for RGP";
                            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // if the servername doesn't parse as IP, it's probably a host name
                    if (!IPAddress.TryParse(data[0], out o))
                    {
                        // lookup host
                        hostEntry = Dns.GetHostEntry(data[0]);
                        if (hostEntry.AddressList.Length > 0)
                        {
                            if (IPAddress.TryParse(hostEntry.AddressList[0].ToString(), out o)) {
                                // host name resolved, update dbserver with ip address
                                data[0] = hostEntry.AddressList[0].ToString();
                            }
                        }
                    }
                    
                    return data;
                }
            }

            return null;
        }


        // get remote database info
        public static string[,] getRemoteInfo(string[] info, string pin)
        {
            try
            {
                string masterKey = getMasterKey(info[4]);
                string password = decrypt(info[2].Substring(4), masterKey);

                MySqlConnection conn;
                string connectionString = "server=" + info[0] + ";";
                connectionString += "uid=" + info[3] + ";";
                connectionString += "pwd=" + password + ";";
                connectionString += "database=" + info[1] + ";";
                
                // open connection
                conn = new MySqlConnection(connectionString);
                conn.Open();

                // make sure PIN is valid
                string query = "SELECT staff_password FROM customers WHERE customer_type = 'STAFF' AND staff_password != ''";
                MySqlCommand cmd = cmd = new MySqlCommand(query, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool match = false;
                string pw;
                while (dataReader.Read())
                {
                    pw = dataReader["staff_password"].ToString();
                    if (decrypt(pw.Substring(4), masterKey) == pin)
                    {
                        match = true;
                        break;
                    }
                }
                dataReader.Close();

                if (!match)
                {
                    MessageBox.Show("Invalid PIN", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                

                // get number of remote locations
                query = "SELECT COUNT(*) FROM remote_databases";
                cmd = new MySqlCommand(query, conn);
                dataReader = cmd.ExecuteReader();
                int count = 0;
                while (dataReader.Read())
                {
                    count = dataReader.GetInt32(0);
                }
                dataReader.Close();

                // get remote database info
                IPAddress o;
                IPHostEntry hostEntry;
                string[,] result = new string[count+1, 5];
                query = "SELECT * FROM remote_databases";
                cmd = new MySqlCommand(query, conn);
                dataReader = cmd.ExecuteReader();
                int x = 0;
                while (dataReader.Read())
                {
                    result[x,0] = dataReader["host"].ToString();
                    result[x,1] = dataReader["dbname"].ToString();
                    result[x,2] = dataReader["pswd"].ToString();
                    result[x,3] = dataReader["user"].ToString();
                    result[x,4] = dataReader["name"].ToString();
                    
                    // if the servername doesn't parse as IP, it's probably a host name
                    if (!IPAddress.TryParse(result[x, 0], out o))
                    {
                        // lookup host
                        hostEntry = Dns.GetHostEntry(result[x, 0]);
                        if (hostEntry.AddressList.Length > 0)
                        {
                            if (IPAddress.TryParse(hostEntry.AddressList[0].ToString(), out o))
                            {
                                // host name resolved, update dbserver with ip address
                                result[x, 0] = hostEntry.AddressList[0].ToString();
                            }
                        }
                    }

                    x++;
                }
                dataReader.Close();

                // get current connection location name
                query = "SELECT value " +
                        "FROM settings " +
                        "WHERE name = 'LocalDatabaseHumanName' " +
                        "LIMIT 1";
                cmd = new MySqlCommand(query, conn);
                dataReader = cmd.ExecuteReader();
                dataReader.Read();

                // add in the current connection to the list
                result[x, 0] = info[0].ToString();
                result[x, 1] = info[1].ToString();
                result[x, 2] = info[2].ToString();
                result[x, 3] = info[3].ToString();
                result[x, 4] = dataReader["value"].ToString();

                dataReader.Close();

                // close Connection
                conn.Close();
                
                return result;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }



        // select the new remote based on the ip address being in the same subnet
        public string[][] findNewRemote(string[,] remoteInfo, string localIP, string subnetMask, string connectedIP)
        {
            byte[] local = IPAddress.Parse(localIP).GetAddressBytes();
            byte[] mask = IPAddress.Parse(subnetMask).GetAddressBytes();
            byte[] current;
            List<int> matchIndex = new List<int>();
            bool byteMatch = true;

            for (int x = 0; x < remoteInfo.GetLength(0); x++)
            {
                byteMatch = true;
                current = IPAddress.Parse(remoteInfo[x, 0]).GetAddressBytes();
                for (int i = 0; i < 4; i++)
                {
                    if ((local[i] & mask[i]) != (current[i] & mask[i]))
                    {
                        byteMatch = false;
                    }
                }

                if (byteMatch)
                {
                    matchIndex.Add(x);
                }
            }

            if (matchIndex.Count < 1)
            {
                // no match found, check to see if we're already on the correct server
                byteMatch = true;
                current = IPAddress.Parse(connectedIP.Trim()).GetAddressBytes();
                for (int i = 0; i < 4; i++)
                {
                    if ((local[i] & mask[i]) != (current[i] & mask[i]))
                    {
                        byteMatch = false;
                    }
                }

                if (byteMatch)
                {
                    MessageBox.Show("You are already connected to your local RGP server, no need to update the connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("You are not on the same network as any RGP server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                return null;
            }

            // get array of all matching facilities
            // should normally be just one, but in case of
            // multiple servers on the same subnet, we will
            // return all of them
            string[][] result = new string[matchIndex.Count][];
            for(int j = 0; j < matchIndex.Count; j++)
            {
                result[j] = new string[]
                {
                    remoteInfo[matchIndex[j], 0],
                    remoteInfo[matchIndex[j], 1],
                    remoteInfo[matchIndex[j], 2],
                    remoteInfo[matchIndex[j], 3],
                    remoteInfo[matchIndex[j], 4]
                };
            }

            return result;
        }


        // update registry to new settings
        public void updateRegistry(string[] info, string masterKey)
        {
            RegistryKey MyReg = Registry.CurrentUser.CreateSubKey
                            (@"SOFTWARE\VB and VBA Program Settings\RockGymPRO\Config");
            if (MyReg != null)
            {
                MyReg.SetValue("dbserver", info[0]);
                MyReg.SetValue("dbname", info[1]);
                MyReg.SetValue("dbpass", info[2]);
                MyReg.SetValue("dbuser", info[3]);
                MyReg.SetValue("MasterEncryptionKey", masterKey);
                MyReg.Close();
                updateStatus("Success! Have an A1 Day!");
                MessageBox.Show("Rock Gym Pro is now connected to " + info[4], "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // success, close program
                Application.Exit();
            }
            else
            {
                updateStatus("Enter your PIN to configure RGP");
                MessageBox.Show("Couldn't open registry", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // update the status text
        public void updateStatus(string text)
        {
            statusText.Text = text;
        }



        // Network functions


        // get the local ip by staging a UDP connection and looking at the end point
        public static string getLocalIP(string server)
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect(server, 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }



        // get subnet mask of the local ip
        public static string GetSubnetMask(string addr)
        {
            IPAddress address = IPAddress.Parse(addr);
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask.ToString();
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }





        /*************************************
         * 
         * Encryption and protection functions
         * 
         *************************************/
         


        // get master key
        public static string getMasterKey(string encryptedKey)
        {
            return Encoding.UTF8.GetString(
                ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedKey)
                    , null
                    , DataProtectionScope.CurrentUser));
        }


        // decrypt string
        public static string decrypt(string data, string secret)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(secret));
            des.Key = byteHash;
            des.Mode = CipherMode.CBC;
            des.IV = Convert.FromBase64String(data.Substring(0, 12));
            byteBuff = Convert.FromBase64String(data.Substring(12));

            string plaintext = Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;

        }


        // protect master key with Windows DPAPI
        public static string Protect(string stringToEncrypt)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(stringToEncrypt)
                    , null
                    , DataProtectionScope.CurrentUser));
        }

        // encryption for storing dbpass in registry
        public static string encrypt(string data, string secret)
        {
            byte[] clear;

            var encoding = new UTF8Encoding();
            var md5 = new MD5CryptoServiceProvider();

            byte[] key = md5.ComputeHash(encoding.GetBytes(secret));
            byte[] iv = new byte[8];
            for (int i = 0; i < 8; i++)
                iv[i] = Convert.ToByte(i); // just some dummy value

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = key;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            des.IV = iv;

            byte[] input = encoding.GetBytes(data);
            try { clear = des.CreateEncryptor(des.Key, des.IV).TransformFinalBlock(input, 0, input.Length); }
            finally
            {
                des.Clear();
                md5.Clear();
            }

            return Convert.ToBase64String(iv) + Convert.ToBase64String(clear);
        }
    }
}
