using GemBox.Pdf;
using GemBox.Pdf.Forms;
using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PDF_Console_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            using (var document = PdfDocument.Load(@"/root/SignTest/PDFTest/Hello World.pdf"))
            {
                // Add an invisible signature field to the PDF document.
                var signatureField = document.Form.Fields.AddSignature();

                // Initiate signing of a PDF file with the specified signer delegate.
                signatureField.Sign(pdfFileStream =>
                {
                    // Create a signed CMS object using the content that should be signed,
                    // but not included in the signed CMS object (detached: true).
                    var content = new byte[pdfFileStream.Length];
                    pdfFileStream.Read(content, 0, content.Length);
                    var signedCms = new SignedCms(new ContentInfo(content), detached: true);

                    X509Certificate2 certificate = null;
                    try
                    {
                        // Compute the signature using the specified digital ID file and the password.
                        certificate = new X509Certificate2(@"/root/SignTest/PDFTest/GemBoxExampleExplorer.pfx", "GemBoxPassword");
                        var cmsSigner = new CmsSigner(certificate);
                        cmsSigner.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1"); // SHA256
                        signedCms.ComputeSignature(cmsSigner);
                    }
                    finally
                    {
                        // Starting with the .NET Framework 4.6, this type implements the IDisposable interface.
                        (certificate as IDisposable)?.Dispose();
                    }

                    // Return the signature encoded into a CMS/PKCS #7 message.
                    return signedCms.Encode();

                }, PdfSignatureFormat.PKCS7, 2199);

                // Finish signing of a PDF file.
                document.Save(@"/root/SignTest/Digital-Signature.pdf");
            }
        }
    }
}
