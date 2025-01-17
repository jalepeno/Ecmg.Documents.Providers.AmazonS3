using Documents.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Providers.AmazonS3.Provider
{
  public partial class S3Provider : IRepositoryDiscovery
  {

    #region IRepositoryDiscovery Implementation

    public RepositoryIdentifiers GetRepositories()
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
