using Amazon.S3;
using Amazon.S3.Model;
using Documents.Exceptions;
using Documents.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Documents.Providers.AmazonS3.Provider
{
  public partial class S3Provider : CProvider
  {

    #region Class Constants

    private const string PROVIDER_NAME = "Amazon S3 Provider";
    private const string PROVIDER_SYSTEM_TYPE = "Amazon Web Services S3";
    private const string PROVIDER_COMPANY_NAME = "Amazon";
    private const string PROVIDER_PRODUCT_NAME = "Amazon Web Services Simple Storage Service";
    private const string PROVIDER_PRODUCT_VERSION = "5.5";

    private const string BUCKET_NAME = "BucketName";
    private const string ACCESS_KEY = "AccessKey";
    private const string SECRET_KEY = "SecretKey";
    private const string USER_NAME = "UserName";
    private const string PASSWORD = "Password";

    #endregion

    #region Class Variables

    private ProviderSystem _providerSystem = new ProviderSystem(PROVIDER_NAME, PROVIDER_SYSTEM_TYPE, PROVIDER_COMPANY_NAME,
                         PROVIDER_PRODUCT_NAME, PROVIDER_PRODUCT_VERSION);

    private string _serverName;
    private string _url;
    private int _portNumber;
    private bool _trustedConnection;
    private string _objectStoreName;

    private AmazonS3Client _s3Client;
    private bool disposedValue;

    #endregion

    #region Public Properties

    public string BucketName { get; set; }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    #endregion

    #region Constructors

    public S3Provider()
    {
      try
      {
        AddProperties();
      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re-throw the exception to the caller
        throw;
      }
    }

    public S3Provider(string connectionString)
    {
      try
      {
        AddProperties();
        //ParseConnectionString();
      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re-throw the exception to the caller
        throw;
      }
    }

    #endregion

    #region Public Methods

    #region CProvider Implementation

    public override void Connect(ContentSource contentSource)
    {
      try
      {
        //base.Connect(ContentSource);
        InitializeProvider(contentSource);
        InitializeProperties();

        _s3Client = new AmazonS3Client(AccessKey, SecretKey);

        IsConnected = TestConnection();

        if (!IsConnected)
        {
          SetState(ProviderConnectionState.Unavailable);
          ApplicationLogging.WriteLogEntry($"Login to '{Name}' failed.", MethodBase.GetCurrentMethod(), TraceEventType.Error, 200);
          throw new RepositoryNotAvailableException(Name);
        }

      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re - throw the exception to the caller
        throw;
      }
    }


    public override ISearch Search => throw new System.NotImplementedException();

    public override string FolderDelimiter => throw new System.NotImplementedException();

    public override bool LeadingFolderDelimiter => throw new System.NotImplementedException();

    public override ISearch CreateSearch()
    {
      throw new System.NotImplementedException();
    }

    public override IFolder GetFolder(string lpFolderPath, long lpMaxContentCount)
    {
      throw new System.NotImplementedException();
    }

    #endregion

    #endregion

    #region Private Methods

    #region Provider Identification

    private void AddProperties()
    {
      try
      {

        // Add the 'BucketName' property
        ProviderProperties.Add(new ProviderProperty(BUCKET_NAME, typeof(string), true, string.Empty, 3, string.Empty, false, true));

        //// Add the 'Port' property
        //ProviderProperties.Add(new ProviderProperty(PORT_NUMBER, typeof(string), true, "9443", 8, string.Empty, false, true));

        //  Add the 'AccessKey' property
        ProviderProperties.Add(new ProviderProperty(ACCESS_KEY, typeof(string), true, string.Empty, 5, string.Empty, false, true));

        //  Add the 'SecretKey' property
        ProviderProperties.Add(new ProviderProperty(SECRET_KEY, typeof(string), true, string.Empty, 6, string.Empty, false, true));

        //  Add the 'UserName' property
        ProviderProperties.Add(new ProviderProperty(USER_NAME, typeof(string), false, string.Empty, 7, string.Empty, false, true));

        //  Add the 'Password' property
        ProviderProperties.Add(new ProviderProperty(PASSWORD, typeof(string), false, string.Empty, 8, string.Empty, false, true));

        ////  Add the 'ObjectStore' property
        //ProviderProperties.Add(new ProviderProperty(OBJECT_STORE, typeof(string), true, string.Empty, 8, string.Empty, true, true));

        // Sort the provider properties by the sequence number
        ProviderProperties.Sort(new ProviderProperties.ProviderPropertySequenceComparer());

      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re-throw the exception to the caller
        throw;
      }
    }

    #endregion


    private static string GetFolderPathFromItemPath(string itemPath)
    {
      try
      {
        int lastSlashPosition = itemPath.LastIndexOf('/');
        if (lastSlashPosition != -1)
        {
          return itemPath.Substring(0, lastSlashPosition);
        }
        else
        {
          return string.Empty;
        }
      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re - throw the exception to the caller
        throw;
      }
    }

    private void InitializeProperties()
    {
      try
      {
        foreach (ProviderProperty property in ProviderProperties)
        {
          switch (property.PropertyName)
          {
            case BUCKET_NAME:
              {
                BucketName = property.PropertyValue.ToString();
                break;
              }

            case ACCESS_KEY:
              {
                AccessKey = property.PropertyValue.ToString();
                break;
              }

            case SECRET_KEY:
              {
                SecretKey = property.PropertyValue.ToString();
                break;
              }
            
            case USER_NAME:
              {
                UserName = property.PropertyValue.ToString();
                break;
              }

            case PASSWORD:
              {
                ProviderPassword = property.PropertyValue.ToString();
                break;
              }

            default:
              break;
          }
        }
      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        //  Re - throw the exception to the caller
        throw;
      }
    }

    private bool TestConnection()
    {
      try
      {
        List<string> returnList = ListRootFolders();
        return true;
      }
      catch (Exception ex)
      {
        ApplicationLogging.LogException(ex, MethodBase.GetCurrentMethod());
        return false;
      }
    }

    private List<string> ListRootFolders()
    {
      List<string> returnList = new List<string>();

      ListObjectsV2Request request = new ListObjectsV2Request()
      {
        BucketName = BucketName
      };

      Task<ListObjectsV2Response> response = _s3Client.ListObjectsV2Async(request);
      response.Wait();

      foreach (S3Object obj in response.Result.S3Objects)
      {
        //Console.WriteLine($"Object Key: {obj.Key}");
        returnList.Add(obj.Key);
      }

      return returnList;

    }

    #endregion

  }
}
