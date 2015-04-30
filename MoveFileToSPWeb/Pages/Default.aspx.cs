﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Hosting;
using System.IO;

namespace MoveFileToSPWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                Response.Write(clientContext.Web.Title);
            }
        }

        protected void btnMove_Click(object sender, EventArgs e)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                //ctx.Web.UploadDocumentToLibrary(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), "Docs", true);
                //lblStatus1.Text = "Document has been uploaded to host web to new library called Docs, which was created unless it already existed.";

                using (var fs = new FileStream(HostingEnvironment.MapPath(string.Format("~/{0}", "Files/Bill.txt")), FileMode.Open))
           {
               var fi = new FileInfo("Bill");
               var list = clientContext.Web.Lists.GetByTitle("FileDocLib");
               clientContext.Load(list.RootFolder);
               clientContext.ExecuteQuery();
               var fileUrl = String.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, fi.Name);

               Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileUrl, fs, true);
            }
       }
            }
        
    }
}