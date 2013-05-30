//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: comm.proto
namespace comm
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MessageType")]
  public partial class MessageType : global::ProtoBuf.IExtensible
  {
    public MessageType() {}
    
    [global::ProtoBuf.ProtoContract(Name=@"Type")]
    public enum Type
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"REPLY", Value=100)]
      REPLY = 100,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SIGNUP", Value=101)]
      SIGNUP = 101,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SIGNIN", Value=102)]
      SIGNIN = 102,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SIGNOUT", Value=103)]
      SIGNOUT = 103,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER_INFO_GET", Value=104)]
      USER_INFO_GET = 104,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER_INFO_SET", Value=105)]
      USER_INFO_SET = 105,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER_FORGOT_PASS", Value=106)]
      USER_FORGOT_PASS = 106,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER_CHANGE_PASS", Value=107)]
      USER_CHANGE_PASS = 107,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FOLLOW_UNFOLLOW", Value=108)]
      FOLLOW_UNFOLLOW = 108,
            
      [global::ProtoBuf.ProtoEnum(Name=@"USER_REL_ARTIST", Value=109)]
      USER_REL_ARTIST = 109,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOTIFICATION", Value=110)]
      NOTIFICATION = 110,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ARTIST_CREATE", Value=111)]
      ARTIST_CREATE = 111,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ARTIST_INFO_GET", Value=112)]
      ARTIST_INFO_GET = 112,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ARTIST_INFO_SET", Value=113)]
      ARTIST_INFO_SET = 113,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ARTIST_LIST_USER_FOLLOW", Value=114)]
      ARTIST_LIST_USER_FOLLOW = 114,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ARTIST_LIST_EXISTED", Value=115)]
      ARTIST_LIST_EXISTED = 115,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_CREATE", Value=116)]
      MOTION_CREATE = 116,
            
      [global::ProtoBuf.ProtoEnum(Name=@"OPEN_MOTION_PART", Value=117)]
      OPEN_MOTION_PART = 117,
            
      [global::ProtoBuf.ProtoEnum(Name=@"WRITE_MOTION_DATA", Value=118)]
      WRITE_MOTION_DATA = 118,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CLOSE_MOTION_PART", Value=119)]
      CLOSE_MOTION_PART = 119,
            
      [global::ProtoBuf.ProtoEnum(Name=@"CANCEL_CREATE_MOTION", Value=120)]
      CANCEL_CREATE_MOTION = 120,
            
      [global::ProtoBuf.ProtoEnum(Name=@"POST", Value=121)]
      POST = 121,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DOWNLOAD_MOTION", Value=122)]
      DOWNLOAD_MOTION = 122,
            
      [global::ProtoBuf.ProtoEnum(Name=@"READ_MOTION_DATA", Value=123)]
      READ_MOTION_DATA = 123,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_UPDATE_INFO", Value=124)]
      MOTION_UPDATE_INFO = 124,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_UPDATE_DATA", Value=125)]
      MOTION_UPDATE_DATA = 125,
            
      [global::ProtoBuf.ProtoEnum(Name=@"COMPLETE_UPDATE", Value=126)]
      COMPLETE_UPDATE = 126,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_INFO_GET", Value=127)]
      MOTION_INFO_GET = 127,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_OF_ARTIST", Value=128)]
      MOTION_OF_ARTIST = 128,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_DOWNLOAD_USER", Value=129)]
      MOTION_DOWNLOAD_USER = 129,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_OF_CATEGORY", Value=130)]
      MOTION_OF_CATEGORY = 130,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_SEARCH", Value=131)]
      MOTION_SEARCH = 131,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MOTION_VERSION", Value=132)]
      MOTION_VERSION = 132,
            
      [global::ProtoBuf.ProtoEnum(Name=@"RATING", Value=133)]
      RATING = 133,
            
      [global::ProtoBuf.ProtoEnum(Name=@"RATING_MOTION_INFO", Value=134)]
      RATING_MOTION_INFO = 134,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ADD_MOTION_FEATURE", Value=135)]
      ADD_MOTION_FEATURE = 135,
            
      [global::ProtoBuf.ProtoEnum(Name=@"REM_MOTION_FEATURE", Value=136)]
      REM_MOTION_FEATURE = 136,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NUMBER_MOTION_OF_CATEGORY", Value=137)]
      NUMBER_MOTION_OF_CATEGORY = 137,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NUMBER_ARTIST", Value=138)]
      NUMBER_ARTIST = 138,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NUMBER_MOTION_OF_ARTIST", Value=139)]
      NUMBER_MOTION_OF_ARTIST = 139,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NUMBER_MOTION_SEARCH", Value=140)]
      NUMBER_MOTION_SEARCH = 140,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NUMBER_MOTION_DOWNLOAD", Value=141)]
      NUMBER_MOTION_DOWNLOAD = 141,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NUMBER_MOTION_RATING_INFO", Value=142)]
      NUMBER_MOTION_RATING_INFO = 142,
            
      [global::ProtoBuf.ProtoEnum(Name=@"RATING_INFO_USER", Value=143)]
      RATING_INFO_USER = 143,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LIST_INFO_MOTION", Value=144)]
      LIST_INFO_MOTION = 144
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Reply")]
  public partial class Reply : global::ProtoBuf.IExtensible
  {
    public Reply() {}
    
    private int _type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int type
    {
      get { return _type; }
      set { _type = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"Type")]
    public enum Type
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"OK", Value=0)]
      OK = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"IO_ERROR", Value=1)]
      IO_ERROR = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DB_ERROR", Value=2)]
      DB_ERROR = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"NO_PERMISSION", Value=3)]
      NO_PERMISSION = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}