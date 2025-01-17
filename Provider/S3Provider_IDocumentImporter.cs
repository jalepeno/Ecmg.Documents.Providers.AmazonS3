using Documents.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Providers.AmazonS3.Provider
{
  public partial class S3Provider : IDocumentImporter
  {

    #region IDocumentImporter Implementation

    public bool EnforceClassificationCompliance => throw new NotImplementedException();

    public event DocumentImportedEventHandler DocumentImported;
    public event DocumentImportErrorEventHandler DocumentImportError;
    public event DocumentImportMessageEventHandler DocumentImportMessage;

    public bool ImportDocument(ref ImportDocumentArgs Args)
    {
      throw new NotImplementedException();
    }

    public void OnDocumentImported(ref DocumentImportedEventArgs e)
    {
      throw new NotImplementedException();
    }

    public void OnDocumentImportError(ref DocumentImportErrorEventArgs e)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
