using Amazon.S3.Model;
using Documents.Arguments;
using Documents.Core;
using Documents.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Providers.AmazonS3.Provider
{
  public partial class S3Provider : IDocumentExporter
  {

    #region IDocumentExporter Implementation

    public event DocumentExportEventHandler DocumentExported;
    public event FolderDocumentExportEventHandler FolderDocumentExported;
    public event FolderExportedEventHandler FolderExported;
    public event DocumentExportErrorEventHandler DocumentExportError;
    public event DocumentExportMessageEventHandler DocumentExportMessage;

    public long DocumentCount(string lpFolderPath, RecursionLevel lpRecursionLevel = RecursionLevel.ecmThisLevelOnly)
    {
      throw new NotImplementedException();
    }

    public bool ExportDocument(string lpId)
    {
      try
      {
        return ExportDocument(new ExportDocumentEventArgs(lpId));
      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re - throw the exception to the caller
        throw;
      }
    }

    public bool ExportDocument(ExportDocumentEventArgs Args)
    {
      try
      {

        Document document = new Document() { DocumentClass = "S3Document", ID = Args.Id };

        Core.Version primaryVersion = document.CreateVersion();

        if (document.Versions.Count == 0) { document.Versions.Add(primaryVersion); }

        string fileName = Path.GetFileName(Args.Id);
        string baseFileName = Path.GetFileNameWithoutExtension(Args.Id);
        string folderPath = GetFolderPathFromItemPath(Args.Id);

        if (baseFileName != null)
        {
          primaryVersion.SetPropertyValue("FileName", baseFileName, true);
        }
        else
        {
          primaryVersion.SetPropertyValue("FileName", fileName, true);
        }       

        if (!string.IsNullOrEmpty(folderPath)) { document.AddFolderPath(folderPath); }

        GetObjectRequest request = new GetObjectRequest() { BucketName = BucketName, Key = Args.Id }; // <-- in S3 key represents a path

        Task<GetObjectResponse> response = _s3Client.GetObjectAsync(request);
        response.Wait();

        MemoryStream returnStream = new MemoryStream();

        Task copyTask = response.Result.ResponseStream.CopyToAsync(returnStream);
        copyTask.Wait();

        NamedStream contentStream = new NamedStream(returnStream, fileName);
        Content content = new Content(contentStream);
        primaryVersion.Contents.Add(content);

        Args.Document = document;

        return ExportDocumentComplete(this, Args);

      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re - throw the exception to the caller
        throw;
      }
    }

    public void OnDocumentExported(ref DocumentExportedEventArgs e)
    {
      DocumentExported?.Invoke(this, ref e);
    }

    public void OnDocumentExportError(ref DocumentExportErrorEventArgs e)
    {
      DocumentExportError?.Invoke(this, e);
    }

    public void OnDocumentExportMessage(ref WriteMessageArgs e)
    {
      DocumentExportMessage?.Invoke(this, e);
    }

    public void OnFolderDocumentExported(ref FolderDocumentExportedEventArgs e)
    {
      FolderDocumentExported?.Invoke(this, ref e);
    }

    public void OnFolderExported(ref FolderExportedEventArgs e)
    {
      FolderExported?.Invoke(this, ref e);
    }

    #endregion

  }
}
