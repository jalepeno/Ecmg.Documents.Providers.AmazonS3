using Documents.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Providers.AmazonS3.Provider
{
  public partial class S3Provider : IClassification
  {

    #region IClassification Implementation

    public ClassificationProperties ContentProperties => throw new NotImplementedException();

    public DocumentClasses DocumentClasses => throw new NotImplementedException();

    public DocumentClass get_DocumentClass(string lpDocumentClassName)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
