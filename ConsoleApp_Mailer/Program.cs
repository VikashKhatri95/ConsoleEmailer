using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Console_Mailer
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Downloaded Hall Tickets";
            string connectionString = "Data Source=40.113.245.190,50891;Initial Catalog=ARTL;User ID=sa;Password=CMS@123;Connect Timeout=0;Pooling=True;Max Pool Size=1000;";
            string emailFrom = "mcastudent615@gmail.com";
            string password = "rmgt hprp zvjk nqjq"; // Use a secure method to handle passwords

            List<string> folderNames = GetFolderNames(path);

            foreach (var folderName in folderNames)
            {
                string folderPath = Path.Combine(path, folderName);
                string attachmentPath = GetAttachmentPath(folderPath);

                if (attachmentPath != null)
                {
                    DataTable resultTable = GetEmailFromFolderName(folderName, connectionString);
              
                    foreach (DataRow row in resultTable.Rows)
                    {
                        string cleanedName = row["DataValue"].ToString();
                        string email = row["Email"].ToString();
                        string subject = "Email sender using Console";
                        string body = $"Dear Sir/Mam,<br><br>Please find the Hall Ticket attached with URN number: {cleanedName}.";
                        SendEmail(email, subject, body, attachmentPath, emailFrom, password);
                        // Now you can use cleanedName and email as needed
                    }
                }
                else
                {
                    Console.WriteLine($"No attachment found in folder: {folderName}");
                }
            }
        }

        static List<string> GetFolderNames(string path)
        {
            List<string> folderNames = new List<string>();

            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    folderNames.Add(Path.GetFileName(directory));
                }
            }
            else
            {
                Console.WriteLine("The specified path does not exist.");
            }

            return folderNames;
        }

        static DataTable GetEmailFromFolderName(string uniqueUserId, string connectionString)
        {
            DataTable resultTable = new DataTable();
            string cleanedString = string.Empty;
            string email = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("MatchUrnEmail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UniqueUserId", uniqueUserId));

                    try
                    {
                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }

            return resultTable;
        }
          

        static void SendEmail(string emailTo, string subject, string body, string attachmentPath, string emailFrom, string password)
        {
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                if (File.Exists(attachmentPath))
                {
                    Attachment attachment = new Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);
                }
                else
                {
                    Console.WriteLine("Attachment file not found.");
                    return;
                }

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    try
                    {
                        smtp.Send(mail);
                        Console.WriteLine($"Email sent successfully to {emailTo}.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email to {emailTo}: {ex.Message}");
                    }
                }
            }
        }

        static string GetAttachmentPath(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                // Assuming there is only one file to attach in each folder
                string[] files = Directory.GetFiles(folderPath);
                if (files.Length > 0)
                {
                    return files[0]; // Return the first file found
                }
            }

            return null;
        }
    }
}



//using System;
//using System.Net;
//using System.Net.Mail;
//using System.IO;


//namespace Console_Mailer
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            string smtpAddress = "smtp.gmail.com";//smtp.office365.com"
//            int portNumber = 587; // or use 465
//            bool enableSSL = true;

//            //string emailFrom = "keshavjagtap1998@gmail.com";
//            //string password = "hkar ttmw gfgq yopg";//rmgt hprp zvjk nqjq
//            //string emailTo = "keshav.jagtap@krishmark.com";//"usha.rathor@krishmark.com";
//            string emailFrom = "mcastudent615@gmail.com";
//            string password = "rmgt hprp zvjk nqjq";//rmgt hprp zvjk nqjq
//            string emailTo = "vikash.khatri@krishmark.com";//"usha.rathor@krishmark.com";
//            string emailcc = "usha.rathor@krishmark.com";
//            string subject = "Email sender using Console";
//            //string body = "Dear Sir/Mam,<br><br>Please find the Resume attached.";
//            string body = "Dear Sir/Mam,<br><br>Please find the Hall Ticket attached.";
//            string attachmentPath = @"F:\Vikash\Vikash_K_Resume.pdf"; // Change the path to your resume

//            using (MailMessage mail = new MailMessage())
//            {
//                mail.From = new MailAddress(emailFrom);
//                mail.To.Add(emailTo);
//                mail.To.Add(emailcc);
//                mail.Subject = subject;
//                mail.Body = body;
//                mail.IsBodyHtml = true; // Can be set to false if the email body is plain text

//                if (File.Exists(attachmentPath))
//                {
//                    Attachment attachment = new Attachment(attachmentPath);
//                    mail.Attachments.Add(attachment);
//                }
//                else
//                {
//                    Console.WriteLine("Attachment file not found.");
//                    return;
//                }

//                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
//                {
//                    smtp.Credentials = new NetworkCredential(emailFrom, password);
//                    smtp.EnableSsl = enableSSL;
//                    try
//                    {
//                        smtp.Send(mail);
//                        Console.WriteLine("Email sent successfully.");
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Error: {ex.Message}");
//                    }
//                }
//            }
//        }
//    }
//}

//using System;
//using System.Net;
//using System.Net.Mail;
//using System.IO;
//using System.Collections.Generic;

//namespace Console_Mailer
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            string smtpAddress = "smtp.gmail.com"; //smtp.office365.com"
//            int portNumber = 587; // or use 465
//            bool enableSSL = true;

//            //string emailFrom = "keshavjagtap1998@gmail.com";
//            //string password = "hkar ttmw gfgq yopg";//rmgt hprp zvjk nqjq
//            //string emailTo = "keshav.jagtap@krishmark.com";//"usha.rathor@krishmark.com";
//            string emailFrom = "mcastudent615@gmail.com";
//            string password = "rmgt hprp zvjk nqjq";//rmgt hprp zvjk nqjq
//            string emailTo = "vikash.khatri@krishmark.com";//"usha.rathor@krishmark.com";
//            string emailcc = "vikas.singh@krishmark.com";
//            string subject = "Email sender using Console";
//            //string body = "Dear Sir/Mam,<br><br>Please find the Resume attached.";
//            string body = "Dear Sir/Mam,<br><br>Please find the Hall Ticket attached.";
//            string attachmentPath = @"F:\Vikash\Vikash_K_Resume.pdf"; // Change the path to your resume

//            using (MailMessage mail = new MailMessage())
//            {
//                mail.From = new MailAddress(emailFrom);
//                mail.To.Add(emailTo);
//                mail.To.Add(emailcc);
//                mail.Subject = subject;
//                mail.Body = body;
//                mail.IsBodyHtml = true; // Can be set to false if the email body is plain text

//                if (File.Exists(attachmentPath))
//                {
//                    Attachment attachment = new Attachment(attachmentPath);
//                    mail.Attachments.Add(attachment);
//                }
//                else
//                {
//                    Console.WriteLine("Attachment file not found.");
//                    return;
//                }

//                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
//                {
//                    smtp.Credentials = new NetworkCredential(emailFrom, password);
//                    smtp.EnableSsl = enableSSL;
//                    try
//                    {
//                        smtp.Send(mail);
//                        Console.WriteLine("Email sent successfully.");
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Error: {ex.Message}");
//                    }
//                }
//            }

//            // Example usage of the GetFolderNames method
//            string path = @"C:\Users\Downloaded Hall Tickets";
//            List<string> folderNames = GetFolderNames(path);

//            Console.WriteLine("Folders in path:");
//            foreach (var folderName in folderNames)
//            {
//                Console.WriteLine(folderName);
//            }
//        }

//        static List<string> GetFolderNames(string path)
//        {
//            List<string> folderNames = new List<string>();

//            if (Directory.Exists(path))
//            {
//                string[] directories = Directory.GetDirectories(path);
//                foreach (string directory in directories)
//                {
//                    folderNames.Add(Path.GetFileName(directory));
//                }
//            }
//            else
//            {
//                Console.WriteLine("The specified path does not exist.");
//            }

//            return folderNames;
//            8B107B9CC7934A4EB6EC4528083431A4_2
//        }
//    }
//}


//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Net;
//using System.Net.Mail;
//namespace Console_Mailer
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            string path = @"C:\Users\Downloaded Hall Tickets";
//            List<string> folderNames = GetFolderNames(path);

//            foreach (var folderName in folderNames)
//            {
//                string connectionString = "Data Source=40.113.245.190,50891;Initial Catalog=ARTL;User ID=sa;Password=CMS@123;Connect Timeout=0;Pooling=True; Max Pool Size=1000;";

//                (string cleanedName, string email) = GetEmailFromFolderName(folderName, connectionString);
//                //(string cleanedName, string email) = MatchUrnEmail("8B107B9CC7934A4EB6EC4528083431A4_2");
//                Console.WriteLine(email);
//                if (!string.IsNullOrEmpty(email))
//                {
//                    string folderPath = Path.Combine(path, folderName);
//                    string attachmentPath = GetAttachmentPath(folderPath);
//                   // SendEmail(email, cleanedName);
//                }
//            }
//        }

//        static List<string> GetFolderNames(string path)
//        {
//            List<string> folderNames = new List<string>();

//            if (Directory.Exists(path))
//            {
//                string[] directories = Directory.GetDirectories(path);
//                foreach (string directory in directories)
//                {
//                    folderNames.Add(Path.GetFileName(directory));
//                }
//            }
//            else
//            {
//                Console.WriteLine("The specified path does not exist.");
//            }

//            return folderNames;
//        }

//        static (string cleanedName, string email) GetEmailFromFolderName(string folderName, string connectionString)
//        {
//            string cleanedString = string.Empty;
//            string email = string.Empty;

//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                using (SqlCommand command = new SqlCommand("GetEmailFromFolderName", connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.Parameters.Add(new SqlParameter("@folderName", folderName));

//                    SqlParameter cleanedNameParameter = new SqlParameter
//                    {
//                        ParameterName = "@cleanedName",
//                        SqlDbType = SqlDbType.NVarChar,
//                        Size = 255,
//                        Direction = ParameterDirection.Output
//                    };
//                    command.Parameters.Add(cleanedNameParameter);

//                    SqlParameter emailParameter = new SqlParameter
//                    {
//                        ParameterName = "@email",
//                        SqlDbType = SqlDbType.NVarChar,
//                        Size = 255,
//                        Direction = ParameterDirection.Output
//                    };
//                    command.Parameters.Add(emailParameter);

//                    try
//                    {
//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        cleanedString = cleanedNameParameter.Value.ToString();
//                        email = emailParameter.Value.ToString();
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Error: {ex.Message}");
//                    }
//                }
//            }

//            return (cleanedString, email);
//        }

//        static void SendEmail(string emailTo, string cleanedName)
//        {
//            string smtpAddress = "smtp.gmail.com";
//            int portNumber = 587;
//            bool enableSSL = true;
//            string emailFrom = "mcastudent615@gmail.com";
//            string password = "rmgt hprp zvjk nqjq";
//            string attachmentPath = @"F:\Vikash\Vikash_K_Resume.pdf";
//            string emailcc = "vikash.khatri@krishmark.com";
//            string subject = "Email sender using Console";
//            string body = $"Dear Sir/Mam,<br><br>Please find the cleaned name: {cleanedName}.";

//            using (MailMessage mail = new MailMessage())
//            {


//                if (File.Exists(attachmentPath))
//                {
//                    Attachment attachment = new Attachment(attachmentPath);
//                    mail.Attachments.Add(attachment);
//                }
//                else
//                {
//                    Console.WriteLine("Attachment file not found.");
//                    return;
//                }

//                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
//                {
//                    smtp.Credentials = new NetworkCredential(emailFrom, password);
//                    smtp.EnableSsl = enableSSL;
//                    try
//                    {
//                        smtp.Send(mail);
//                        Console.WriteLine($"Email sent successfully to {emailTo}.");
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Error sending email to {emailTo}: {ex.Message}");
//                    }
//                }
//            }
//        }

//        static string GetAttachmentPath(string folderPath)
//        {
//            if (Directory.Exists(folderPath))
//            {
//                // Assuming there is only one file to attach in each folder
//                string[] files = Directory.GetFiles(folderPath);
//                if (files.Length > 0)
//                {
//                    return files[0]; // Return the first file found
//                }
//            }

//            return null;
//        }

//    }
//}
