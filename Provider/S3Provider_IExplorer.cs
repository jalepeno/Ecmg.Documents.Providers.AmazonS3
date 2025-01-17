using Documents.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Providers.AmazonS3.Provider
{
  public partial class S3Provider : IExplorer
  {

    #region IExplorer Implementation
    public IFolder RootFolder => throw new NotImplementedException();

    public IFolder get_GetFolderByID(string lpFolderID, int lpFolderLevels, int lpMaxContentCount)
    {
      throw new NotImplementedException();
    }

    public FolderContents get_GetFolderContentsByID(string lpFolderID, int lpMaxContentCount)
    {
      throw new NotImplementedException();
    }

    public bool get_HasSubFolders(string lpFolderID)
    {
      throw new NotImplementedException();
    }

    public bool get_IsFolderValid(string lpFolderID)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
