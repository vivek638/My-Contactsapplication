using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Xml;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace My_Contacts
{
    public partial class MyContactsMainForm : Form
    {
        String x86folder = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public string StorageDirectory = null;
        public string UserFirstName = null, UserLastName = null, UserAddress = null, UserCellNo = null, UserHomeNo = null, UserOfficeNo = null, UserOtherNo = null, UserEmailAddress = null, UserFacebookLink = null, UserNotes=null, UserName = null, UserPicture64=null,UserDirectory = null;
        Image UserPicture = null;

        bool AllContactsPanelSelected = false, SearchContactsPanelSelected = false, AddContactsPanelSelected = false, ExportContactsPanelSelected = false, ImportContactsPanelSelected = false, HowToUsePanelSelected = false, AboutPanelSelected = false, ShowContactDetailsPanelSelected=false , LoginPanelSelected = false, CreateAccountPanelSelected = false;

        string ContactFirstName = null, ContactFirstNameFinal = null, ContactLastName = null, ContactLastNameFinal = null, ContactAddress = null, ContactCellNumber = null, ContactCellNumberFinal=null, ContactHomeNumber = null, ContactOfficeNumber = null, ContactEmail = null, ContactOtherNumber = null, ContactRelation = null, ContactFacebookLink = null, ContactNotes = null, ContactPicture64 = null;
        Image ContactPicture = null;

        string SelectedContactFile = null;

        string ImportingFilePath=null, ImportContactFileSaveName= null;

        string FPUserName = null, FPUserEmail = null;
        string FPRealEmail = null, FPRealPassword = null;
        string FPUserDirectory = null;

        public MyContactsMainForm()
        {
            InitializeComponent();
        }

        private void MyContactsMainForm_Load(object sender, EventArgs e)
        {
            try
            {
                StorageDirectoryCreator();
                UserPicCircularSetter();

                UserName = "default"; //on first loading before anyone have logged in

                ReadUserDetails();
                LoadUserDetails();

                Falser(8);
                SelectedColorer();
                ForgotPasswordConditionHider();
                LoginPanel.BringToFront();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "You are logged in as " + UserName + " user. Login to your account or Create new.";
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : MCFL -" + exc.Message;
            }
        }
        private void StorageDirectoryCreator()
        {
            try
            {
                StorageDirectory = Directory.GetCurrentDirectory() + @"\My Contacts\Data\Profiles\";
                if (!Directory.Exists(StorageDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(StorageDirectory);
                        Directory.CreateDirectory(StorageDirectory + @"\default\Details\");
                        Directory.CreateDirectory(StorageDirectory + @"\default\Contacts\");

                        try
                        {
                            DefaultUserDetailWriter();
                        }
                        catch
                        {

                        }
                    }
                    catch
                    {
                        MessageBox.Show("Sorry !! We can not create My Contacts Storage on this computer. Error : 77", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot access storage folder.";
            }

            //StorageDirectory = x86folder + @"\Sajjad Gull\My Contacts\Data\Profiles\";

            //if (!Directory.Exists(StorageDirectory))
            //{
            //    if (!Directory.Exists(StorageDirectory))
            //    {
            //        StorageDirectory = @"D:\My Contacts\Data\Profiles\";

            //        if (!Directory.Exists(StorageDirectory))
            //        {
            //            StorageDirectory = @"E:\My Contacts\Data\Profiles\";
            //            if (!Directory.Exists(StorageDirectory))
            //            {
            //                try
            //                {
            //                    StorageDirectory = x86folder + @"\Sajjad Gull\My Contacts\Data\Profiles\";
            //                    Directory.CreateDirectory(StorageDirectory);
            //                }
            //                catch
            //                {
            //                    try
            //                    {
            //                        StorageDirectory = @"D:\My Contacts\Data\Profiles\";
            //                        Directory.CreateDirectory(StorageDirectory);
            //                    }
            //                    catch
            //                    {
            //                        try
            //                        {
            //                            StorageDirectory = @"E:\My Contacts\Data\Profiles\";
            //                            Directory.CreateDirectory(StorageDirectory);
            //                        }
            //                        catch
            //                        {
            //                            MessageBox.Show("Sorry !! My Contacts is not able to create Data storage on this computer.");
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
        private void DefaultUserDetailWriter()
        {
            try
            {
                FileStream fs = new FileStream(StorageDirectory + @"\default\Details\default.xml", FileMode.Create, FileAccess.ReadWrite);

                using (XmlWriter writer = XmlWriter.Create(fs))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("AccountDetail");

                    writer.WriteElementString("AccountFirstName", "Default");
                    writer.WriteElementString("AccountLastName", "User");
                    writer.WriteElementString("AccountAddress", "Karachi, Pakistan");
                    writer.WriteElementString("AccountCellNumber", "+923320307817");
                    writer.WriteElementString("AccountHomeNumber", "none");
                    writer.WriteElementString("AccountOfficeNumber", "none");
                    writer.WriteElementString("AccountEmail", "quickmoon@rocketmail.com");
                    writer.WriteElementString("AccountPassword", "Be6pjvShP7QnNooG8gZOHZsXe0Gm+IAWRcSiM4btTLaGzW5L0cxZtDXifGmS2LFkB3Agzg==");
                    writer.WriteElementString("AccountUserName", "default");
                    writer.WriteElementString("AccountFacebookLink", "https://www.facebook.com/founderofWSC");
                    writer.WriteElementString("AccountNotes", "Setting up My Contacts. test account for default user.");
                    writer.WriteElementString("AccountPicture", "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");

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                fs.Close();
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : DUDW -" + exc.Message;
            }
        }
        private void UserPicCircularSetter()
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, UserPictureBox.Width - 3, UserPictureBox.Height - 3);
            Region rg = new Region(gp);
            UserPictureBox.Region = rg;
        }
        private void ReadUserDetails()
        {
            try
            {
                UserDirectory = StorageDirectory + UserName + @"\";

                FileStream fs = new FileStream(UserDirectory + @"Details\" + UserName + ".xml", FileMode.Open, FileAccess.Read);

                StreamReader sr = new StreamReader(fs);

                using (XmlTextReader AccountReader = new XmlTextReader(fs))
                {
                    while (AccountReader.Read())
                    {
                        if (AccountReader.NodeType == XmlNodeType.Element)
                        {
                            if (AccountReader.LocalName.Equals("AccountFirstName"))
                            {
                                UserFirstName = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountLastName"))
                            {
                                UserLastName = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountAddress"))
                            {
                                UserAddress = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountCellNumber"))
                            {
                                UserCellNo = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountHomeNumber"))
                            {
                                UserHomeNo = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountOfficeNumber"))
                            {
                                UserOfficeNo = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountEmail"))
                            {
                                UserEmailAddress = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountFacebookLink"))
                            {
                                UserFacebookLink = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountNotes"))
                            {
                                UserNotes = AccountReader.ReadString();
                            }
                            if (AccountReader.LocalName.Equals("AccountPicture"))
                            {
                                //UserPicture64 = AccountReader.ReadString();
                                byte[] imageBytes1 = Convert.FromBase64String(AccountReader.ReadString());
                                MemoryStream ms = new MemoryStream(imageBytes1, 0, imageBytes1.Length);
                                ms.Write(imageBytes1, 0, imageBytes1.Length);
                                Image img1 = Image.FromStream(ms, true);
                                UserPicture = img1;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : RUD -" + exc.Message;
            }
        }
        private void LoadUserDetails()
        {
            UserFirstNameLabel.Text = UserFirstName;
            UserLastNameLabel.Text = UserLastName;
            UserPictureBox.Image = UserPicture;
        }
        private void SelectedColorer()
        {
            if (AllContactsPanelSelected)
            {
                AllContactsLabelPanelBluer();
                //AllContactsPanelSelected = false;
            }
            else if (!AllContactsPanelSelected)
            {
                AllContactsLabelPanelWhiter();
            }
            if (SearchContactsPanelSelected)
            {
                SearchContactLabelPanelBluer();
                //SearchContactsPanelSelected = false;
            }
            else if (!SearchContactsPanelSelected)
            {
                SearchContactLabelPanelWhiter();
            }
            if (AddContactsPanelSelected)
            {
                AddNewContactLabelPanelBluer();
                //AddContactsPanelSelected = false;
            }
            else if (!AddContactsPanelSelected)
            {
                AddNewContactLabelPanelWhiter();
            }
            if (ExportContactsPanelSelected)
            {
                ExportContactLabelPanelBluer();
                //ExportContactsPanelSelected = false;
            }
            else if (!ExportContactsPanelSelected)
            {
                ExportContactLabelPanelWhiter();
            }
            if (ImportContactsPanelSelected)
            {
                ImportContactLabelPanelBluer();
                //ImportContactsPanelSelected = false;
            }
            else if (!ImportContactsPanelSelected)
            {
                ImportContactLabelPanelWhiter();
            }
            if (HowToUsePanelSelected)
            {
                HowToUseLabelPanelBluer();
                //HowToUsePanelSelected = false;
            }
            else if (!HowToUsePanelSelected)
            {
                HowToUseLabelPanelWhiter();
            }
            if (AboutPanelSelected)
            {
                AboutLabelPanelBluer();
                //AboutPanelSelected = false;
            }
            else if (!AboutPanelSelected)
            {
                AboutLabelPanelWhiter();
            }
            if (LoginPanelSelected)
            {
                LoginLabelPanelBlacker();
            }
            else if (!LoginPanelSelected)
            {
                LoginLabelPanelBluer();
            }
            if (CreateAccountPanelSelected)
            {
                CreateAccountPanelBlacker();
            }
            else if (!CreateAccountPanelSelected)
            {
                CreateAccountPanelBluer();
            }
        }
        private void Falser(int PanelNo)
        {
            if (PanelNo == 0)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            if (PanelNo == 1)
            {
                AllContactsPanelSelected = true;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 2)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = true;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 3)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = true;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 4)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = true;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 5)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = true;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 6)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = true;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 7)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = true;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 8)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = true;
                CreateAccountPanelSelected = false;
            }
            else if (PanelNo == 9)
            {
                AllContactsPanelSelected = false;
                SearchContactsPanelSelected = false;
                AddContactsPanelSelected = false;
                ExportContactsPanelSelected = false;
                ImportContactsPanelSelected = false;
                HowToUsePanelSelected = false;
                AboutPanelSelected = false;
                LoginPanelSelected = false;
                CreateAccountPanelSelected = true;
            }
        }
        private void RefreshContacts()
        {
            try
            {
                AllContactsListBox.Items.Clear();

                foreach (string f in Directory.GetFiles(UserDirectory + @"Contacts\", "*.xml"))
                {
                    FileInfo fyle = new FileInfo(f);
                    string ActualFileName = "";

                    for (int i = 0; i < fyle.Name.Length - 4; i++)
                    {
                        ActualFileName = ActualFileName + fyle.Name[i];
                    }

                    AllContactsListBox.Items.Add(ActualFileName);
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : RCs -" + exc.Message;
            }
        }
        private void RefreshContactsforSearchPanel()
        {
            try
            {
                ContactsResultListBox.Items.Clear();

                foreach (string f in Directory.GetFiles(UserDirectory + @"Contacts\", "*.xml"))
                {
                    FileInfo fyle = new FileInfo(f);
                    string ActualFileName = "";

                    for (int i = 0; i < fyle.Name.Length - 4; i++)
                    {
                        ActualFileName = ActualFileName + fyle.Name[i];
                    }

                    ContactsResultListBox.Items.Add(ActualFileName);
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : RCFSP -" + exc.Message;
            }
        }
        private void RefreshExportContactsListBox()
        {
            try
            {
                ExportContactsListBox.Items.Clear();

                foreach (string f in Directory.GetFiles(UserDirectory + @"Contacts\", "*.xml"))
                {
                    FileInfo fyle = new FileInfo(f);
                    string ActualFileName = "";

                    for (int i = 0; i < fyle.Name.Length - 4; i++)
                    {
                        ActualFileName = ActualFileName + fyle.Name[i];
                    }

                    ExportContactsListBox.Items.Add(ActualFileName);
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : RECLB -" + exc.Message;
            }
        }
        private void ReadContactFile()
        {
            try
            {
                using (XmlTextReader ContactReader = new XmlTextReader(UserDirectory + @"\Contacts\" + SelectedContactFile))
                {
                    while (ContactReader.Read())
                    {
                        if (ContactReader.NodeType == XmlNodeType.Element)
                        {
                            if (ContactReader.LocalName.Equals("ContactFirstName"))
                            {
                                ContactFirstName = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactLastName"))
                            {
                                ContactLastName = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactAddress"))
                            {
                                ContactAddress = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactCellNumber"))
                            {
                                ContactCellNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactHomeNumber"))
                            {
                                ContactHomeNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactOfficeNumber"))
                            {
                                ContactOfficeNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactEmail"))
                            {
                                ContactEmail = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactOtherNumber"))
                            {
                                ContactOtherNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactRelation"))
                            {
                                ContactRelation = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactFacebookLink"))
                            {
                                ContactFacebookLink = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactNotes"))
                            {
                                ContactNotes = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactPicture"))
                            {
                                byte[] imageBytes1 = Convert.FromBase64String(ContactReader.ReadString());
                                MemoryStream ms = new MemoryStream(imageBytes1, 0, imageBytes1.Length);
                                ms.Write(imageBytes1, 0, imageBytes1.Length);
                                Image img1 = Image.FromStream(ms, true);
                                ContactPicture = img1;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : RCF -" + exc.Message;
            }
        }
        private void LoadContactDetails()
        {
            try
            {
                ResultFirstNameLabel.Text = ContactFirstName;
                ResultLastNameLabel.Text = ContactLastName;
                ResultAddressRichBox.Text = ContactAddress;
                ResultCellNumberLabel.Text = ContactCellNumber;
                ResultHomeNumberLabel.Text = ContactHomeNumber;
                ResultOfficeNumberLabel.Text = ContactOfficeNumber;
                ResultEmailLabel.Text = ContactEmail;
                ResultOtherNumberLabel.Text = ContactOtherNumber;
                ResultRelationLabel.Text = ContactRelation;
                ResultFacebookLinkLabel.Text = ContactFacebookLink;
                ResultNotesRichBox.Text = ContactNotes;
                ResultContactPictureBox.Image = ContactPicture;

                ContactDetailShowerPanel.BringToFront();
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : LCD -" + exc.Message;
            }
        }
        private void LoadContactDetailEditor()
        {
            try
            {
                FirstNameBox.Text = ContactFirstName;
                LastNameBox.Text = ContactLastName;
                AddressRichBox.Text = ContactAddress;
                CellNoBox.Text = ContactCellNumber;
                HomeNoBox.Text = ContactHomeNumber;
                OfficeNoBox.Text = ContactOfficeNumber;
                EmailBox.Text = ContactEmail;
                OtherNoBox.Text = ContactOtherNumber;
                RelationBox.Text = ContactRelation;
                FacebookLinkBox.Text = ContactFacebookLink;
                AddNotesRichBox.Text = ContactNotes;
                ContactPictureBox.Image = ContactPicture;

                AddNewContactPanel.BringToFront();
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : LCDE -" + exc.Message;
            }
        }
        private void RefreshingAddContactPanel()
        {
            FirstNameBox.Text= null;
            LastNameBox.Text= null;
            AddressRichBox.Text= null;
            CellNoBox.Text= null;
            HomeNoBox.Text= null;
            OfficeNoBox.Text= null;
            EmailBox.Text= null;
            OtherNoBox.Text= null;
            RelationBox.Text= null;
            FacebookLinkBox.Text= null;
            AddNotesRichBox.Text= null;
            ContactPictureBox.Image = global::My_Contacts.Properties.Resources.ContactSamplePic;
        }
        private void RefreshContactsResultPanel()
        {
            ResultFirstNameLabel.Text = null;
            ResultLastNameLabel.Text = null;
            ResultAddressRichBox.Text = null;
            ResultCellNumberLabel.Text = null;
            ResultHomeNumberLabel.Text = null;
            ResultOfficeNumberLabel.Text = null;
            ResultEmailLabel.Text = null;
            ResultOtherNumberLabel.Text = null;
            ResultRelationLabel.Text = null;
            ResultFacebookLinkLabel.Text = null;
            ResultNotesRichBox.Text = null;
            ResultContactPictureBox.Image = null;
        }
        private void ContactFileAnalyzer(string ImportContactFilePath)
        {
            try
            {
                string ICFContactFirstName = null, ICFContactLastName = null, ICFContactAddress = null, ICFContactCellNumber = null, ICFContactHomeNumber = null, ICFContactOfficeNumber = null, ICFContactEmail = null, ICFContactOtherNumber = null, ICFContactRelation = null, ICFContactFacebookLink = null, ICFContactNotes = null, ICFContactPicture64 = null;
                Image ICFContactPicture = null;

                bool ICFNameCondition = false, ICFCellNumberCondition = false;

                using (XmlTextReader ContactReader = new XmlTextReader(ImportContactFilePath))
                {
                    while (ContactReader.Read())
                    {
                        if (ContactReader.NodeType == XmlNodeType.Element)
                        {
                            if (ContactReader.LocalName.Equals("ContactFirstName"))
                            {
                                ICFContactFirstName = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactLastName"))
                            {
                                ICFContactLastName = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactAddress"))
                            {
                                ICFContactAddress = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactCellNumber"))
                            {
                                ICFContactCellNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactHomeNumber"))
                            {
                                ICFContactHomeNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactOfficeNumber"))
                            {
                                ICFContactOfficeNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactEmail"))
                            {
                                ICFContactEmail = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactOtherNumber"))
                            {
                                ICFContactOtherNumber = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactRelation"))
                            {
                                ICFContactRelation = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactFacebookLink"))
                            {
                                ICFContactFacebookLink = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactNotes"))
                            {
                                ICFContactNotes = ContactReader.ReadString();
                            }
                            if (ContactReader.LocalName.Equals("ContactPicture"))
                            {
                                byte[] imageBytes1 = Convert.FromBase64String(ContactReader.ReadString());
                                MemoryStream ms = new MemoryStream(imageBytes1, 0, imageBytes1.Length);
                                ms.Write(imageBytes1, 0, imageBytes1.Length);
                                Image img1 = Image.FromStream(ms, true);
                                ICFContactPicture = img1;
                            }
                        }
                    }
                }

                ////MessageBox.Show(ICFContactFirstName + ",\n" + ICFContactLastName + ",\n" + ICFContactAddress + ",\n" + ICFContactCellNumber + ",\n" + ICFContactHomeNumber + ",\n" + ICFContactOfficeNumber + ",\n" + ICFContactEmail + ",\n" + ICFContactFacebookLink + ",\n" + ICFContactRelation);

                if ((ICFContactFirstName + ICFContactLastName) != "")
                {
                    ICFNameCondition = true;
                    ConditionContainBasicInformationLabel.Text = "Contains Necessary Name details.";
                    this.ConditionContainBasicInformationPictureBox.Image = global::My_Contacts.Properties.Resources.DoneGreen;
                    ConditionContainBasicInformationLabel.Show();
                    ConditionContainBasicInformationPictureBox.Show();
                }
                else if ((ICFContactFirstName + ICFContactLastName) == "")
                {
                    ICFNameCondition = false;
                    ConditionContainBasicInformationLabel.Text = "Doesn't contains Necessary Name details !!!";
                    this.ConditionContainBasicInformationPictureBox.Image = global::My_Contacts.Properties.Resources.ExitRed;
                    ConditionContainBasicInformationLabel.Show();
                    ConditionContainBasicInformationPictureBox.Show();
                }
                if ((ICFContactCellNumber) != "")
                {
                    ICFCellNumberCondition = true;
                    ConditionContainCellNoLabel.Text = "Contains Necessary Cell Number details.";
                    this.ConditionContainCellNoPictureBox.Image = global::My_Contacts.Properties.Resources.DoneGreen;
                    ConditionContainCellNoLabel.Show();
                    ConditionContainCellNoPictureBox.Show();
                }
                else if ((ICFContactCellNumber) == "")
                {
                    ICFCellNumberCondition = false;
                    ConditionContainCellNoLabel.Text = "Doesn't contains Necessary Cell Number details.";
                    this.ConditionContainCellNoPictureBox.Image = global::My_Contacts.Properties.Resources.ExitRed;
                    ConditionContainCellNoLabel.Show();
                    ConditionContainCellNoPictureBox.Show();
                }
                if ((ICFContactAddress != "") || (ICFContactHomeNumber != ""))
                {
                    ConditionContainAddressHomeNoLabel.Text = "Contains address and home number details.";
                    this.ConditionContainAddressHomeNoPictureBox.Image = global::My_Contacts.Properties.Resources.DoneGreen;
                    ConditionContainAddressHomeNoLabel.Show();
                    ConditionContainAddressHomeNoPictureBox.Show();
                }
                else if ((ICFContactCellNumber == "") || (ICFContactHomeNumber == ""))
                {
                    ConditionContainAddressHomeNoLabel.Text = "Doesn't contains address and home number details.";
                    this.ConditionContainAddressHomeNoPictureBox.Image = global::My_Contacts.Properties.Resources.WarnYellow;
                    ConditionContainAddressHomeNoLabel.Show();
                    ConditionContainAddressHomeNoPictureBox.Show();
                }
                if ((ICFContactEmail != "") || (ICFContactOfficeNumber != ""))
                {
                    ConditionContainEmailOfficeNoLabel.Text = "Contains email address and office number details.";
                    this.ConditionContainEmailOfficeNoPictureBox.Image = global::My_Contacts.Properties.Resources.DoneGreen;
                    ConditionContainEmailOfficeNoLabel.Show();
                    ConditionContainEmailOfficeNoPictureBox.Show();
                }
                else if ((ICFContactEmail == "") || (ICFContactOfficeNumber == ""))
                {
                    ConditionContainEmailOfficeNoLabel.Text = "Doesn't contains email address and office number details.";
                    this.ConditionContainEmailOfficeNoPictureBox.Image = global::My_Contacts.Properties.Resources.WarnYellow;
                    ConditionContainEmailOfficeNoLabel.Show();
                    ConditionContainEmailOfficeNoPictureBox.Show();
                }
                if ((ICFContactPicture != null))
                {
                    ConditionContainPictureLabel.Text = "Contains Picture.";
                    this.ConditionContainPicturePictureBox.Image = global::My_Contacts.Properties.Resources.DoneGreen;
                    ConditionContainPictureLabel.Show();
                    ConditionContainPicturePictureBox.Show();
                }
                else if ((ICFContactPicture == null))
                {
                    ConditionContainPictureLabel.Text = "Doesn't contains Picture.";
                    this.ConditionContainPicturePictureBox.Image = global::My_Contacts.Properties.Resources.WarnYellow;
                    ConditionContainPictureLabel.Show();
                    ConditionContainPicturePictureBox.Show();
                }
                if ((ICFContactRelation != "") || (ICFContactFacebookLink != ""))
                {
                    ConditionContainRelationFacebookLabel.Text = "Contains Relation and Facebook link details";
                    this.ConditionContainRelationFacebookPictureBox.Image = global::My_Contacts.Properties.Resources.DoneGreen;
                    ConditionContainRelationFacebookLabel.Show();
                    ConditionContainRelationFacebookPictureBox.Show();
                }
                else if ((ICFContactRelation == "") || (ICFContactFacebookLink == ""))
                {
                    ConditionContainRelationFacebookLabel.Text = "Doesn't contains Relation and Facebook link details";
                    this.ConditionContainRelationFacebookPictureBox.Image = global::My_Contacts.Properties.Resources.WarnYellow;
                    ConditionContainRelationFacebookLabel.Show();
                    ConditionContainRelationFacebookPictureBox.Show();
                }

                //checking basic condition. if done.. proceed to import button enableing

                if ((ICFNameCondition) && (ICFCellNumberCondition))
                {
                    ImportContactFileSaveName = ICFContactFirstName + " " + ICFContactLastName + " - " + ICFContactCellNumber + ".xml";
                    StatusMessageDoner();
                    StatusMessageLabel.Text = "Basic conditions fullfilled. You can now import this contact.";
                    ImportContactButton.Enabled = true;
                }
                else if ((!ICFNameCondition) || (!ICFCellNumberCondition))
                {
                    StatusMessageError();
                    StatusMessageLabel.Text = "Doesn't fullfill necessary conditions. You can not import this contact.";
                    ImportContactButton.Enabled = false;
                }

                AnalyzeHiderPanel.Show();
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : CFA -" + exc.Message;
            }
        }
        private void AnalyzeHider()
        {
            ImportContactFilePathBox.Text = null;
            AnalyzeHiderPanel.Hide();
            ImportContactButton.Enabled = false;
            ConditionContainBasicInformationPictureBox   . Hide();
            ConditionContainBasicInformationLabel        . Hide();
            ConditionContainCellNoPictureBox             . Hide();
            ConditionContainCellNoLabel                  . Hide();
            ConditionContainAddressHomeNoPictureBox      . Hide();
            ConditionContainAddressHomeNoLabel           . Hide();
            ConditionContainEmailOfficeNoPictureBox      . Hide();
            ConditionContainEmailOfficeNoLabel           . Hide();
            ConditionContainPicturePictureBox            . Hide();
            ConditionContainPictureLabel                 . Hide();
            ConditionContainRelationFacebookPictureBox   . Hide();
            ConditionContainRelationFacebookLabel.Hide();
        }
        private void ForgotPasswordConditionHider()
        {
            FPUserNameLabel.Hide();
            FPUserNameBox.Hide();
            FPEmailLabel.Hide();
            FPEmailBox.Hide();
            FPRecoverPasswordButton.Hide();
        }
        private void ForgotPasswordConditionShower()
        {
            FPUserNameLabel.Show();
            FPUserNameBox.Show();
            FPEmailLabel.Show();
            FPEmailBox.Show();
            FPRecoverPasswordButton.Show();
        }
        private void AllContactsLabelPanelBluer()
        {
            AllContactsLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            AllContactsLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainAllContactsPictureBox.Image = global::My_Contacts.Properties.Resources.AllContactsBlue;
        }
        private void AllContactsLabelPanelWhiter()
        {
            AllContactsLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            AllContactsLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainAllContactsPictureBox.Image = global::My_Contacts.Properties.Resources.AllContactsWhite;
        }
        private void SearchContactLabelPanelBluer()
        {
            SearchContactLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            SearchContactLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainSearchContactPictureBox.Image = global::My_Contacts.Properties.Resources.SearchContactBlue;
        }
        private void SearchContactLabelPanelWhiter()
        {
            SearchContactLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            SearchContactLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainSearchContactPictureBox.Image = global::My_Contacts.Properties.Resources.SearchContactWhite;
        }
        private void AddNewContactLabelPanelBluer()
        {
            AddNewContactLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            AddNewContactLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainAddNewContactPictureBox.Image = global::My_Contacts.Properties.Resources.AddContactBlue;
        }
        private void AddNewContactLabelPanelWhiter()
        {
            AddNewContactLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            AddNewContactLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainAddNewContactPictureBox.Image = global::My_Contacts.Properties.Resources.AddContactWhite;
        }
        private void ExportContactLabelPanelBluer()
        {
            ExportContactLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            ExportContactLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainExportContactsPictureBox.Image = global::My_Contacts.Properties.Resources.ExportContactBlue;
        }
        private void ExportContactLabelPanelWhiter()
        {
            ExportContactLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            ExportContactLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainExportContactsPictureBox.Image = global::My_Contacts.Properties.Resources.ExportContactWhite;
        }
        private void ImportContactLabelPanelBluer()
        {
            ImportContactLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            ImportContactLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainImportContactsPictureBox.Image = global::My_Contacts.Properties.Resources.ImportContactBlue;
        }
        private void ImportContactLabelPanelWhiter()
        {
            ImportContactLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            ImportContactLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainImportContactsPictureBox.Image = global::My_Contacts.Properties.Resources.ImportContactWhite;
        }
        private void HowToUseLabelPanelBluer()
        {
            HowToUseLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            HowToUseLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainHowToUsePictureBox.Image = global::My_Contacts.Properties.Resources.HowToUseBlue;
        }
        private void HowToUseLabelPanelWhiter()
        {
            HowToUseLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            HowToUseLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainHowToUsePictureBox.Image = global::My_Contacts.Properties.Resources.HowToUseWhite;
        }
        private void AboutLabelPanelBluer()
        {
            AboutLabelPanel.BackColor = Color.FromArgb(37, 40, 40);
            AboutLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.MainAboutPictureBox.Image = global::My_Contacts.Properties.Resources.AboutMyContactsBlue;
        }
        private void AboutLabelPanelWhiter()
        {
            AboutLabelPanel.BackColor = Color.FromArgb(46, 52, 52);
            AboutLabel.ForeColor = Color.FromArgb(255, 255, 255);
            this.MainAboutPictureBox.Image = global::My_Contacts.Properties.Resources.AboutMyContactsWhite;
        }
        private void LoginLabelPanelBlacker()
        {
            LoginLabel.ForeColor = Color.FromArgb(46, 52, 52);
            this.LoginControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.LoginBlack;
        }
        private void LoginLabelPanelBluer()
        {
            LoginLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.LoginControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.LoginBlue;
        }
        private void CreateAccountPanelBlacker()
        {
            CreateAccountLabel.ForeColor = Color.FromArgb(46, 52, 52);
            this.CreateAccountControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.CreateAccountBlack;
        }
        private void CreateAccountPanelBluer()
        {
            CreateAccountLabel.ForeColor = Color.FromArgb(120, 180, 242);
            this.CreateAccountControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.CreateAccountBlue;
        }
        private void StatusMessageDefaulter()
        {
            StatusMessageLabel.ForeColor = Color.FromArgb(46, 52, 52);
            this.StatusMessagePanel.BackgroundImage = global::My_Contacts.Properties.Resources.AboutBlack;
        }
        private void StatusMessageError()
        {
            StatusMessageLabel.ForeColor = Color.FromArgb(255, 0, 0);
            this.StatusMessagePanel.BackgroundImage = global::My_Contacts.Properties.Resources.ExitRed;
        }
        private void StatusMessageWarner()
        {
            //this.label34.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            StatusMessageLabel.ForeColor = Color.FromArgb(255, 255, 0);
            StatusMessageLabel.Font = new Font(StatusMessageLabel.Font, FontStyle.Bold);
            this.StatusMessagePanel.BackgroundImage = global::My_Contacts.Properties.Resources.WarnYellow;
        }
        private void StatusMessageDoner()
        {
            StatusMessageLabel.ForeColor = Color.FromArgb(0, 255,0);
            this.StatusMessagePanel.BackgroundImage = global::My_Contacts.Properties.Resources.DoneGreen;
        }
        private void LoginPanelRefresher()
        {
            ForgotPasswordConditionHider();
            LoginUsernameBox.Text = null;
            LoginPasswordBox.Text = null;
        }
        private void ACPanelRefresher()
        {
            CAUserFirstNameBox.Text     = null;
            CAUserLastNameBox.Text      = null;
            CAUserAddressRichBox.Text   = null;
            CAUserCellNoBox.Text        = null;
            CAUserHomeNoBox.Text        = null;
            CAUserOfficeNoBox.Text      = null;
            CAUserEmailBox.Text         = null;
            CAUserPasswordBox.Text      = null;
            CAUserUserNameBox.Text      = null;
            CAUserFacebookLinkBox.Text  = null;
            CAUserAddNotesRichBox.Text  = null;
            CAUserPictureBox.Image = global::My_Contacts.Properties.Resources.UserSamplePic;
        }
        private void AllContactsLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            AllContactsLabelPanelBluer();
        }
        private void AllContactsLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!AllContactsPanelSelected)
            {
                AllContactsLabelPanelWhiter();
            }
        }
        private void SearchContactLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            SearchContactLabelPanelBluer();
        }
        private void SearchContactLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!SearchContactsPanelSelected)
            {
                SearchContactLabelPanelWhiter();
            }
        }
        private void AddNewContactLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            AddNewContactLabelPanelBluer();
        }
        private void AddNewContactLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!AddContactsPanelSelected)
            {
                AddNewContactLabelPanelWhiter();
            }
        }
        private void ExportContactLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            ExportContactLabelPanelBluer();
        }
        private void ExportContactLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!ExportContactsPanelSelected)
            {
                ExportContactLabelPanelWhiter();
            }
        }
        private void ImportContactLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            ImportContactLabelPanelBluer();
        }
        private void ImportContactLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!ImportContactsPanelSelected)
            {
                ImportContactLabelPanelWhiter();
            }
        }
        private void HowToUseLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            HowToUseLabelPanelBluer();
        }
        private void HowToUseLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!HowToUsePanelSelected)
            {
                HowToUseLabelPanelWhiter();
            }
        }
        private void AboutLabelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            AboutLabelPanelBluer();
        }
        private void AboutLabelPanel_MouseLeave(object sender, EventArgs e)
        {
            if (!AboutPanelSelected)
            {
                AboutLabelPanelWhiter();
            }
        }
        private void CloseFormPanel_MouseMove(object sender, MouseEventArgs e)
        {
            CloseFormPanel.BackColor = Color.FromArgb(255, 0, 0);
        }
        private void CloseFormPanel_MouseLeave(object sender, EventArgs e)
        {
            CloseFormPanel.BackColor = Color.FromArgb(247, 117, 128);
        }
        private void MinimizeFormPanel_MouseMove(object sender, MouseEventArgs e)
        {
            MinimizeFormPanel.BackColor = Color.FromArgb(0, 0 , 255);
        }
        private void MinimizeFormPanel_MouseLeave(object sender, EventArgs e)
        {
            MinimizeFormPanel.BackColor = Color.FromArgb(120, 180, 242);
        }
        private void LoginLabel_MouseMove(object sender, MouseEventArgs e)
        {
            LoginLabel.ForeColor = Color.FromArgb(46, 52, 52);
            this.LoginControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.LoginBlack;
        }
        private void LoginLabel_MouseLeave(object sender, EventArgs e)
        {
            if (!LoginPanelSelected)
            {
                LoginLabelPanelBluer();
            }
        }
        private void CreateAccountLabel_MouseMove(object sender, MouseEventArgs e)
        {
            CreateAccountLabel.ForeColor = Color.FromArgb(46, 52, 52);
            this.CreateAccountControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.CreateAccountBlack;
        }
        private void CreateAccountLabel_MouseLeave(object sender, EventArgs e)
        {
            if (!CreateAccountPanelSelected)
            {
                CreateAccountPanelBluer();
            }
        }
        private void UserMainInfoPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.EditUserDetailControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.AccountSettingWhite;
        }
        private void UserMainInfoPanel_MouseLeave(object sender, EventArgs e)
        {
            this.EditUserDetailControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.AccountSettingWhite;
        }
        private void EditUserDetailControlPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.EditUserDetailControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.AccountSettingBlue;
        }
        private void EditUserDetailControlPanel_MouseLeave(object sender, EventArgs e)
        {
            this.EditUserDetailControlPanel.BackgroundImage = global::My_Contacts.Properties.Resources.AccountSettingWhite;
        }
        private void CloseFormPanel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to Close this Application ?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (result == DialogResult.No)
            {
                //Do Nothing
            }
        }
        private void MinimizeFormPanel_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void LoginLabel_Click(object sender, EventArgs e)
        {
            LoginPanelRefresher();
            Falser(8);
            SelectedColorer();
            ForgotPasswordConditionHider();
            LoginPanel.BringToFront();

            StatusMessageDefaulter();
            StatusMessageLabel.Text = "Login to another account.";
        }
        private void CreateAccountLabel_Click(object sender, EventArgs e)
        {
            Falser(9);
            SelectedColorer();
            CreateAccountPanel.BringToFront();

            StatusMessageDefaulter();
            StatusMessageLabel.Text = "Create new account.";
        }
        private void AllContactsLabel_Click(object sender, EventArgs e)
        {
            try
            {

                Falser(1);
                SelectedColorer();
                AllContactsPanel.BringToFront();
                RefreshContacts();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "All contacts in " + UserName + "'s My Contacts account. ";
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ACs - "+exc.Message;
            }
        }
        private void SearchContactLabel_Click(object sender, EventArgs e)
        {
            try
            {
                Falser(2);
                SelectedColorer();
                RefreshContactsforSearchPanel();
                SearchContactPanel.BringToFront();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "Search Contacts in your accounts.";
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : SCs -" + exc.Message;
            }
        }
        private void AddNewContactLabel_Click(object sender, EventArgs e)
        {
            try
            {
                Falser(3);
                SelectedColorer();
                AddNewContactPanel.BringToFront();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "Add new contact to your Contacts.";
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ANCs -" + exc.Message;
            }
        }
        private void ExportContactLabel_Click(object sender, EventArgs e)
        {
            try
            {
                Falser(4);
                SelectedColorer();
                RefreshExportContactsListBox();
                ExportContactsPanel.BringToFront();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "Select any contact to export.";
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ECs -" + exc.Message;
            }
        }
        private void ImportContactLabel_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeHider();
                Falser(5);
                SelectedColorer();
                ImportContactsPanel.BringToFront();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "Import Contact to your account";
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ICs -" + exc.Message;
            }
        }
        private void SaveContactButton_Click(object sender, EventArgs e)
        {
            try
            {
                ContactFirstName = FirstNameBox.Text;
                ContactLastName = LastNameBox.Text;
                ContactAddress = AddressRichBox.Text;
                ContactCellNumber = CellNoBox.Text;
                ContactHomeNumber = HomeNoBox.Text;
                ContactOfficeNumber = OfficeNoBox.Text;
                ContactEmail = EmailBox.Text;
                ContactOtherNumber = OtherNoBox.Text;
                ContactRelation = RelationBox.Text;
                ContactFacebookLink = FacebookLinkBox.Text;
                ContactNotes = AddNotesRichBox.Text;

                MemoryStream ms = new MemoryStream();
                ContactPictureBox.Image.Save(ms, ContactPictureBox.Image.RawFormat);
                byte[] a = ms.GetBuffer();
                ms.Close();
                ContactPicture64 = Convert.ToBase64String(a);

                for (int i = 0; i < ContactFirstName.Length; i++)
                {
                    if (ContactFirstName[i] != ' ')
                    {
                        ContactFirstNameFinal = ContactFirstNameFinal + ContactFirstName[i];
                    }
                }

                for (int i = 0; i < ContactLastName.Length; i++)
                {
                    if (ContactLastName[i] != ' ')
                    {
                        ContactLastNameFinal = ContactLastNameFinal + ContactLastName[i];
                    }
                }

                for (int i = 0; i < ContactCellNumber.Length; i++)
                {
                    if (ContactCellNumber[i] != ' ')
                    {
                        ContactCellNumberFinal = ContactCellNumberFinal + ContactCellNumber[i];
                    }
                }

                if (((ContactFirstNameFinal != null) || (ContactLastNameFinal != null)) && (ContactCellNumberFinal != null))
                {
                    FileStream fs = new FileStream(UserDirectory + @"\Contacts\" + ContactFirstName + " " + ContactLastName + " - " + ContactCellNumber + ".xml", FileMode.Create, FileAccess.ReadWrite);

                    using (XmlWriter writer = XmlWriter.Create(fs))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ContactDetail");

                        writer.WriteElementString("ContactFirstName", ContactFirstName);
                        writer.WriteElementString("ContactLastName", ContactLastName);
                        writer.WriteElementString("ContactAddress", ContactAddress);
                        writer.WriteElementString("ContactCellNumber", ContactCellNumber);
                        writer.WriteElementString("ContactHomeNumber", ContactHomeNumber);
                        writer.WriteElementString("ContactOfficeNumber", ContactOfficeNumber);
                        writer.WriteElementString("ContactEmail", ContactEmail);
                        writer.WriteElementString("ContactOtherNumber", ContactOtherNumber);
                        writer.WriteElementString("ContactRelation", ContactRelation);
                        writer.WriteElementString("ContactFacebookLink", ContactFacebookLink);
                        writer.WriteElementString("ContactNotes", ContactNotes);
                        writer.WriteElementString("ContactPicture", ContactPicture64);

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }

                    fs.Close();

                    StatusMessageDoner();
                    StatusMessageLabel.Text = ContactFirstName + " " + ContactLastName + " has been added to your Contacts.";

                    RefreshingAddContactPanel();
                }
                else
                {
                    if ((ContactFirstNameFinal == null) || (ContactLastNameFinal == null))
                    {
                        MessageBox.Show("Please enter contact's name details.", "Enter Details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else //if (ContactCellNumberFinal == null)
                    {
                        MessageBox.Show("Please enter contact's cell number details.", "Enter Details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : SVB -" + exc.Message;
            }
        }
        private void ChangeContactPictureButton_Click(object sender, EventArgs e)
        {
            A:
            /*selecting image*/

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Select Image file..";
            ofd.DefaultExt = ".jpg";
            ofd.Filter = "Media Files|*.jpg;*.png;*.gif;*.bmp;*.jpeg|All Files|*.*";

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                /*if picture selected then load in the picture box*/
                try
                {
                    ContactPictureBox.Load(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Sorry. This format is not supported. Please select another Image.");
                    goto A;
                }
            }
        }
        private void AllContactsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ShowContactDetailRadioButton.Checked)
                {
                    SelectedContactFile = AllContactsListBox.SelectedItem + ".xml";
                    ReadContactFile();
                    LoadContactDetails();
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ACLSIC -" + exc.Message;
            }
        }
        private void EditContactButton_Click(object sender, EventArgs e)
        {
            try
            {
                LoadContactDetailEditor();
                RefreshContactsResultPanel();
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ECBC -" + exc.Message;
            }
        }
        private void DeleteContactButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to Delete this Contact ?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(UserDirectory + @"\Contacts\" + SelectedContactFile);
                    StatusMessageLabel.Text = "Contact Deleted Successfully.";
                    RefreshContactsResultPanel();
                }
                catch (Exception exc)
                {
                    StatusMessageError();
                    StatusMessageLabel.Text = "Cannot perform this action. Error : DLC -" + exc.Message;
                }
            }
            else if (result == DialogResult.No)
            {
                //Do Nothing
            }
        }
        private void SearchItemBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshContactsforSearchPanel();

                StatusMessageWarner();
                StatusMessageLabel.Text = "Searching in Contacts.";

                try
                {
                    string[] AllContactItems = new string[ContactsResultListBox.Items.Count];
                    int j = 0;

                    string SearchTerm = (SearchItemBox.Text).ToUpper();

                    foreach (string ContactItem in ContactsResultListBox.Items)
                    {
                        AllContactItems[j] = ContactItem;
                        j++;
                    }

                    ContactsResultListBox.Items.Clear();

                    for (int i = 0; i < AllContactItems.Length; i++)
                    {
                        if (AllContactItems[i].ToUpper().Contains(SearchTerm))
                        {
                            ContactsResultListBox.Items.Add(AllContactItems[i]);
                        }
                    }

                    StatusMessageDoner();
                    StatusMessageLabel.Text = "These contacts match your search.";
                }
                catch
                {
                    //do nothing ... just chill :p
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform this action. Error : SIBTC -" + exc.Message;
            }
        }
        private void ContactsResultListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (SearchResultShowDetailsRadioButton.Checked)
                {
                    SelectedContactFile = ContactsResultListBox.SelectedItem + ".xml";
                    ReadContactFile();
                    LoadContactDetails();
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : CRLBSIC -" + exc.Message;
            }
        }
        private void ExportContactButton_Click(object sender, EventArgs e)
        {
            try
            {
                //RefreshExportContactsListBox();
                if (ExportContactsListBox.SelectedIndex > -1)
                {
                    SaveFileDialog sfd = new SaveFileDialog();

                    sfd.Title = "Export Contact..";
                    sfd.DefaultExt = ".xml";
                    sfd.Filter = "My Contact Files|*.xml;*.xcd";

                    DialogResult result = sfd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        File.Copy(UserDirectory + @"\Contacts\" + ExportContactsListBox.SelectedItem + ".xml", sfd.FileName + ".xml");
                    }
                }
                else
                {
                    MessageBox.Show("Please select any contact to Export.", "No Selection.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ECB -" + exc.Message;
            }
        }
        private void SelectContactFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyzeHider();

                StatusMessageDefaulter();
                StatusMessageLabel.Text = "Select file.";

                ImportContactButton.Enabled = false;
                /*selecting file*/

                OpenFileDialog ofd = new OpenFileDialog();

                ofd.Title = "Select My Contact file..";
                ofd.DefaultExt = ".xml";
                ofd.Filter = "My Contact Files|*.xml;*.xcd";

                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    StatusMessageDoner();
                    StatusMessageLabel.Text = "Please wait while contact file is analyzed.";
                    ImportContactFilePathBox.Text = ofd.FileName;
                    ImportingFilePath = ofd.FileName;
                    ContactFileAnalyzer(ofd.FileName);
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : SCFB -" + exc.Message;
            }
        }
        private void ImportContactButton_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 1;
                try
                {
                    File.Copy(ImportingFilePath, UserDirectory + @"\Contacts\" + ImportContactFileSaveName);
                    AnalyzeHider();
                    StatusMessageDoner();
                    StatusMessageLabel.Text = "Contact successfully imported.";
                }
                catch
                {
                A:
                    i++;
                    try
                    {
                        File.Copy(ImportingFilePath, UserDirectory + @"\Contacts\" + i + " - " + ImportContactFileSaveName);
                        AnalyzeHider();
                        StatusMessageWarner();
                        StatusMessageLabel.Text = "Similar contacts exists. Importing as " + i + " - " + ImportContactFileSaveName + ".";
                    }
                    catch
                    {
                        goto A;
                    }
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : ICBC -" + exc.Message;
            }
        }
        private void ForgotPasswordLabel_Click(object sender, EventArgs e)
        {
            try
            {
                StatusMessageWarner();
                StatusMessageLabel.Text = "Please enter your correct Username and email to receive recovery details.";
                ForgotPasswordConditionShower();

                MessageBox.Show("Please know that if you have selected option encrypted at signup, you will not receive password recovery in correct format.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : FPLC -" + exc.Message;
            }
        }
        private void LoginButton_Click(object sender, EventArgs e)
        {
            string LoginUserName = LoginUsernameBox.Text;

            try
            {
                string LoginUserPassword = LoginPasswordBox.Text; 
                string LoginRealEmail = null, LoginRealUserName = null, LoginRealPassword = null, LoginAccountFirstName = null, LoginAccountLastName = null;
                string LoginUserDirectory = StorageDirectory + LoginUserName + @"\";

                if (Directory.Exists(LoginUserDirectory))
                {
                    FileStream fs = new FileStream(LoginUserDirectory + @"Details\" + LoginUserName + ".xml", FileMode.Open, FileAccess.Read);

                    using (XmlTextReader AccountReader = new XmlTextReader(fs))
                    {
                        while (AccountReader.Read())
                        {
                            if (AccountReader.NodeType == XmlNodeType.Element)
                            {
                                if (AccountReader.LocalName.Equals("AccountFirstName"))
                                {
                                    LoginAccountFirstName = AccountReader.ReadString();
                                }
                                if (AccountReader.LocalName.Equals("AccountLastName"))
                                {
                                    LoginAccountLastName = AccountReader.ReadString();
                                }
                                if (AccountReader.LocalName.Equals("AccountEmail"))
                                {
                                    LoginRealEmail = AccountReader.ReadString();
                                }
                                if (AccountReader.LocalName.Equals("AccountPassword"))
                                {
                                    LoginRealPassword = AccountReader.ReadString();
                                }
                                if (AccountReader.LocalName.Equals("AccountUserName"))
                                {
                                    LoginRealUserName = AccountReader.ReadString();
                                }
                            }
                        }
                    }

                    bool Condition = false;

                    if (EncryptedPasswordCheckBox.Checked)
                    {
                        Condition = SimpleHash.VerifyHash(LoginUserPassword, "SHA384", LoginRealPassword);
                    }
                    else if (!EncryptedPasswordCheckBox.Checked)
                    {
                        Condition = (LoginRealPassword == LoginUserPassword);
                    }

                    ///do ebgfhbjjn
                    if ((LoginUserName == LoginRealUserName) && (Condition))
                    {
                        DialogResult result = MessageBox.Show("Wellcome " + LoginUserName + ".\n\nYou are now logging in.", "Wellcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (result == DialogResult.OK)
                        {
                            fs.Close();
                            UserName = LoginUserName;

                            ReadUserDetails();
                            LoadUserDetails();

                            AllContactsPanelSelected = true;

                            RefreshContacts();
                            Falser(1);
                            SelectedColorer();

                            AllContactsPanel.BringToFront();
                            LoginPanelRefresher();

                            StatusMessageDoner();
                            StatusMessageLabel.Text = "Successfully logged in. Wellcome " + UserName;

                            Updater.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry Password didn't matched.\n\nPlease Register new account or login with correct Password.", "Password Didn't Matched.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry this user doesn't exists.\n\nPlease Register new account or login with correct Username & Password.", "User Does'nt Exists.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            catch
            {
                MessageBox.Show("Error : Sorry this user doesn't exists.\n\nPlease Register new account or login with correct Username & Password.", "User Does'nt Exists.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        private void FPRecoverPasswordButton_Click(object sender, EventArgs e)
        {
            FPUserName = FPUserNameBox.Text;
            FPUserEmail = FPEmailBox.Text;

            try
            {
                FPUserDirectory = StorageDirectory + FPUserName + @"\";

                if (Directory.Exists(FPUserDirectory))
                {
                    FileStream fs = new FileStream(FPUserDirectory +@"\Details\"+FPUserName + ".xml", FileMode.Open, FileAccess.Read);

                    MessageBox.Show(fs.ToString());

                    using (XmlTextReader AccountReader = new XmlTextReader(fs))
                    {
                        while (AccountReader.Read())
                        {
                            if (AccountReader.NodeType == XmlNodeType.Element)
                            {
                                if (AccountReader.LocalName.Equals("AccountEmail"))
                                {
                                    FPRealEmail = AccountReader.ReadString();
                                }
                                if (AccountReader.LocalName.Equals("AccountPassword"))
                                {
                                    FPRealPassword = AccountReader.ReadString();
                                }
                            }
                        }
                    }

                    if ((FPUserEmail == FPRealEmail))
                    {
                        StatusMessageWarner();
                        StatusMessageLabel.Text = "Please wait while we sent you recovery details on your email.";
                        ForgotPasswordMailer.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Sorry !! Not an assosiated email with this account.", "Not Associated Email.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry this user doesn't exists.\n\nPlease Register for new account.", "User Does'nt Exists.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            catch
            {
                MessageBox.Show("Sorry this user doesn't exists.\n\nPlease Register new account.", "User Does'nt Exists.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        private void ForgotPasswordMailer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(FPUserDirectory + @"pr.ser", FileMode.OpenOrCreate, FileAccess.Read);

                StreamReader sr = new StreamReader(fs);

                string DateFromFile = sr.ReadToEnd();
                fs.Close();

                if (DateFromFile == DateTime.Now.ToString("d"))
                {
                    StatusMessageError();
                    StatusMessageLabel.Text = "Sorry !! You've already requested for password recovery. Please check your email account.\n\nIf you have not received password recovery details please contact Developer.";
                }
                else
                {
                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                        mail.From = new MailAddress("mycontactsclient@gmail.com");
                        mail.To.Add(FPRealEmail);
                        mail.Subject = "Password Recovery for Your Contacts Book Account";
                        mail.Body = "You requested for your account password recovery at " + DateTime.Now + " .\n\nYour Account Password is < " + FPRealPassword + " >.\n\nIf the above password look like combination of long alien language characters then you had choosed to encrypt password at creating account time. We are sorry. we can't help you in this scenerio.\n\nIf you have not requested this then someone tried to get in your account. Please consider saving your data in any reliable source.\n\nMy Contacts Team.\nKarachi, Pakistan.";

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("mycontactsclient@gmail.com", "Password-Is-removed-From-Visual-Studio-File");
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);

                        FileStream fs2 = new FileStream(FPUserDirectory + @"pr.ser", FileMode.OpenOrCreate, FileAccess.ReadWrite);

                        StreamWriter sw = new StreamWriter(fs2);

                        sw.WriteLine(DateTime.Now.ToString("d"));
                        fs2.Flush();
                        fs2.Close();

                        StatusMessageDoner();
                        StatusMessageLabel.Text = "Recovery procedure sent to your email.";
                    }
                    catch
                    {
                        StatusMessageError();
                        StatusMessageLabel.Text = "An error occured while communicating to server. Server or Internet connection might be down. Please Contact Developer.\n\nError Code : ARENSDPC119";
                    }
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : FPMDW -" + exc.Message;
            }
        }
        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            try
            {
                string AccountFirstName = CAUserFirstNameBox.Text;
                string AccountFirstNameFinal = null;
                string AccountLastName = CAUserLastNameBox.Text;
                string AccountLastNameFinal = null;
                string AccountAddress = CAUserAddressRichBox.Text;
                string AccountCellNumber = CAUserCellNoBox.Text;
                string AccountCellNumberFinal = null;
                string AccountHomeNumber = CAUserHomeNoBox.Text;
                string AccountOfficeNumber = CAUserOfficeNoBox.Text;
                string AccountEmail = CAUserEmailBox.Text;
                string AccountEmailFinal = null;
                string AccountPassword = CAUserPasswordBox.Text;
                string AccountPasswordFinal = null;
                string AccountUserName = CAUserUserNameBox.Text;
                string AccountUserNameFinal = null;
                bool AccountUserNameExist = false;
                string AccountFacebookLink = CAUserFacebookLinkBox.Text;
                string AccountFacebookLinkFinal = null;
                string AccountNotes = CAUserAddNotesRichBox.Text;

                MemoryStream ms = new MemoryStream();
                CAUserPictureBox.Image.Save(ms, ContactPictureBox.Image.RawFormat);
                byte[] a = ms.GetBuffer();
                ms.Close();
                string AccountPicture = Convert.ToBase64String(a);

                {//account first name setting : removing extra blank spaces
                    for (int i = 0; i < AccountFirstName.Length; i++)
                    {
                        if (AccountFirstName[i] != ' ')
                        {
                            AccountFirstNameFinal = AccountFirstNameFinal + AccountFirstName[i];
                        }
                        else if ((AccountFirstName[i] == ' '))
                        {
                            try
                            {
                                if (AccountFirstName[i + 1] != ' ')
                                {
                                    AccountFirstNameFinal = AccountFirstNameFinal + AccountFirstName[i];
                                }
                            }
                            catch
                            {
                                //it means first name ended here. it was a last character
                            }
                        }
                    }

                    CAUserFirstNameBox.Text = AccountFirstNameFinal;
                }

                {//account last name setting : removing extra blank spaces
                    for (int i = 0; i < AccountLastName.Length; i++)
                    {
                        if (AccountLastName[i] != ' ')
                        {
                            AccountLastNameFinal = AccountLastNameFinal + AccountLastName[i];
                        }
                        else if ((AccountLastName[i] == ' '))
                        {
                            try
                            {
                                if (AccountLastName[i + 1] != ' ')
                                {
                                    AccountLastNameFinal = AccountLastNameFinal + AccountLastName[i];
                                }
                            }
                            catch
                            {
                                //it means last name ended here. it was a last character
                            }
                        }
                    }
                    CAUserLastNameBox.Text = AccountLastNameFinal;
                }

                {//account cell number setting : removing extra blank spaces
                    for (int i = 0; i < AccountCellNumber.Length; i++)
                    {
                        if (AccountCellNumber[i] != ' ')
                        {
                            AccountCellNumberFinal = AccountCellNumberFinal + AccountCellNumber[i];
                        }
                        else if ((AccountCellNumber[i] == ' '))
                        {
                            try
                            {
                                if (AccountCellNumber[i + 1] != ' ')
                                {
                                    AccountCellNumberFinal = AccountCellNumberFinal + AccountCellNumber[i];
                                }
                            }
                            catch
                            {
                                //it means number ended here. it was a last character
                            }
                        }
                    }
                    CAUserCellNoBox.Text = AccountCellNumberFinal;
                }

                {//account email setting : removing blank spaces
                    for (int i = 0; i < AccountEmail.Length; i++)
                    {
                        if (AccountEmail[i] != ' ')
                        {
                            AccountEmailFinal = AccountEmailFinal + AccountEmail[i];
                        }
                    }
                    CAUserEmailBox.Text = AccountEmailFinal;
                }

                {//account password encryption
                    //SHA 384 Encryption on it if user allowed.... [AccountPasswordFinal]

                    if (UsePasswordEncryptionCheckBox.Checked)
                    {
                        AccountPasswordFinal = SimpleHash.ComputeHash(AccountPassword, "SHA384", null);
                    }
                    else if (!UsePasswordEncryptionCheckBox.Checked)
                    {
                        AccountPasswordFinal = AccountPassword;
                    }
                }

                {//account username setting : removing blank spaces
                    for (int i = 0; i < AccountUserName.Length; i++)
                    {
                        if (AccountUserName[i] != ' ')
                        {
                            AccountUserNameFinal = AccountUserNameFinal + AccountUserName[i];
                        }
                    }

                    if (Directory.Exists(StorageDirectory + AccountUserNameFinal + @"\"))
                    {
                        AccountUserNameExist = true;
                    }

                    CAUserUserNameBox.Text = AccountUserNameFinal;
                }

                {//account facebook link setting : removing blank spaces
                    for (int i = 0; i < AccountFacebookLink.Length; i++)
                    {
                        if (AccountFacebookLink[i] != ' ')
                        {
                            AccountFacebookLinkFinal = AccountFacebookLinkFinal + AccountFacebookLink[i];
                        }
                    }
                    CAUserFacebookLinkBox.Text = AccountFacebookLinkFinal;
                }

                if ((AccountFirstNameFinal == null) || (AccountFirstNameFinal == " ") || (AccountFirstNameFinal == "") ||
                    (AccountCellNumberFinal == null) || (AccountCellNumberFinal == " ") || (AccountCellNumberFinal == "") ||
                    (AccountEmailFinal == null) || (AccountEmailFinal == " ") || (AccountEmailFinal == "") ||
                    (AccountUserNameFinal == null) || (AccountUserNameFinal == " ") || (AccountUserNameFinal == "") ||
                    (AccountPasswordFinal == null) || (AccountPasswordFinal == " ") || (AccountPasswordFinal == "") ||
                    (AccountUserNameExist == true))
                {
                    if ((AccountFirstNameFinal == null) || (AccountFirstNameFinal == " ") || (AccountFirstNameFinal == ""))
                    {
                        MessageBox.Show("Please write valid first or last name. Empty Name not allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if ((AccountCellNumberFinal == null) || (AccountCellNumberFinal == " ") || (AccountCellNumberFinal == ""))
                    {
                        MessageBox.Show("Please write valid Cell Number. Empty Cell Number not allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if ((AccountEmailFinal == null) || (AccountEmailFinal == " ") || (AccountEmailFinal == ""))
                    {
                        MessageBox.Show("Please write valid email address. Empty email not allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if ((AccountUserNameFinal == null) || (AccountUserNameFinal == " ") || (AccountUserNameFinal == ""))
                    {
                        MessageBox.Show("Please write valid username. Empty username not allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if ((AccountPasswordFinal == null) || (AccountPasswordFinal == " ") || (AccountPasswordFinal == ""))
                    {
                        MessageBox.Show("Please write valid Password. Empty Password not allowed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (AccountUserNameExist == true)
                    {
                        MessageBox.Show("This user name already exists on this computer. Please choose another one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if ((AccountLastName == null) || (AccountLastName == "") || (AccountLastName == " "))
                    {
                        AccountLastName = "-";
                    }
                    if ((AccountHomeNumber == null) || (AccountHomeNumber == "") || (AccountHomeNumber == " "))
                    {
                        AccountHomeNumber = "-";
                    }
                    if ((AccountOfficeNumber == null) || (AccountOfficeNumber == "") || (AccountOfficeNumber == " "))
                    {
                        AccountOfficeNumber = "-";
                    }
                    if ((AccountNotes == null) || (AccountNotes == "") || (AccountNotes == " "))
                    {
                        AccountNotes = "-";
                    }
                    if ((AccountFacebookLink == null) || (AccountFacebookLink == "") || (AccountFacebookLink == " "))
                    {
                        AccountFacebookLink = "-";
                    }

                    try
                    {
                        Directory.CreateDirectory(StorageDirectory + AccountUserNameFinal + @"\");
                        Directory.CreateDirectory(StorageDirectory + AccountUserNameFinal + @"\Contacts\");
                        Directory.CreateDirectory(StorageDirectory + AccountUserNameFinal + @"\Details\");
                    

                    FileStream fs = new FileStream(StorageDirectory + AccountUserNameFinal + @"\Details\" + AccountUserNameFinal + ".xml", FileMode.Create, FileAccess.ReadWrite);

                   
                        using (XmlWriter writer = XmlWriter.Create(fs))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("AccountDetail");

                            writer.WriteElementString("AccountFirstName", AccountFirstName);
                            writer.WriteElementString("AccountLastName", AccountLastName);
                            writer.WriteElementString("AccountAddress", AccountAddress);
                            writer.WriteElementString("AccountCellNumber", AccountCellNumber);
                            writer.WriteElementString("AccountHomeNumber", AccountHomeNumber);
                            writer.WriteElementString("AccountOfficeNumber", AccountOfficeNumber);
                            writer.WriteElementString("AccountEmail", AccountEmail);
                            writer.WriteElementString("AccountPassword", AccountPasswordFinal);
                            writer.WriteElementString("AccountUserName", AccountUserName);
                            writer.WriteElementString("AccountFacebookLink", AccountFacebookLink);
                            writer.WriteElementString("AccountNotes", AccountNotes);
                            writer.WriteElementString("AccountPicture", AccountPicture);

                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                        }
                        fs.Close();

                        UserName = AccountUserName;

                        ReadUserDetails();
                        LoadUserDetails();

                        AllContactsPanelSelected = true;

                        RefreshContacts();
                        Falser(1);
                        SelectedColorer();

                        AllContactsPanel.BringToFront();

                        ACPanelRefresher();

                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show("An error occured. Please use as Administrator. SNAD - " + ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception exc)
            {
                StatusMessageError();
                StatusMessageLabel.Text = "Cannot perform action. Error : CABC -" + exc.Message;
            }
        }
        private void CAChangeUserPictureButton_Click(object sender, EventArgs e)
        {
            A:
            /*selecting image*/

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Select Image file..";
            ofd.DefaultExt = ".jpg";
            ofd.Filter = "Media Files|*.jpg;*.png;*.gif;*.bmp;*.jpeg|All Files|*.*";

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                /*if picture selected then load in the picture box*/
                try
                {
                    CAUserPictureBox.Load(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Sorry. This format is not supported. Please select another Image.");
                    goto A;
                }
            }
        }
        private void Updater_DoWork(object sender, DoWorkEventArgs e)
        {
        }
        private void HowToUseLabel_Click(object sender, EventArgs e)
        {
            HowToUsePanel.BringToFront();
            Falser(6);
            SelectedColorer();
            StatusMessageDefaulter();
            StatusMessageLabel.Text = "Information about using My Contacts.";
        }
        private void AboutLabel_Click(object sender, EventArgs e)
        {
            AboutPanel.BringToFront();
            Falser(7);
            SelectedColorer();
            StatusMessageDefaulter();
            StatusMessageLabel.Text = "A little about this application and its Developer.";
        }
        private void ShowContactDetailRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            StatusMessageDoner();
            StatusMessageLabel.Text = "Now contact details will be shown on selection.";
        }
        private void SearchResultShowDetailsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            StatusMessageDoner();
            StatusMessageLabel.Text = "Now contact details will be shown on selection";
        }
        private void UsePasswordEncryptionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UsePasswordEncryptionCheckBox.Checked)
            {
                MessageBox.Show("Please remember that if Password is encrypted than we will not be able to send you recovery password details in case if you forget your password.","Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!UsePasswordEncryptionCheckBox.Checked)
            {
                MessageBox.Show("Please remember that if Password is not encrypted than attackers might be able to read it easily.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SeeVideoDetailsLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.csharpens.com/MyContactsVideo");
        }
        private void SajjadFacebookLinkPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/founderofWSC");
        }
        private void SajjadGooglePlusLinkPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://plus.google.com/+SajjadGull");
        }
        private void SajjadTwitterLinkPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.twitter.com/@Csharpens");
        }
        private void SajjadEmailPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:quickmoon@rocketmail.com");
        }
    }
}
