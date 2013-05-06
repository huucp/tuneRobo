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


    public partial class Reply : global::ProtoBuf.IExtensible
    {


        private artist.CreateArtistReply _create_artist = null;
        [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"create_artist", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public artist.CreateArtistReply create_artist
        {
            get { return _create_artist; }
            set { _create_artist = value; }
        }

        private artist.ArtistInfoReply _artist_info = null;
        [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"artist_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public artist.ArtistInfoReply artist_info
        {
            get { return _artist_info; }
            set { _artist_info = value; }
        }

        private artist.UpdateArtistReply _update_artist = null;
        [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name = @"update_artist", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public artist.UpdateArtistReply update_artist
        {
            get { return _update_artist; }
            set { _update_artist = value; }
        }

        private artist.UserArtistReply _user_artist = null;
        [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name = @"user_artist", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public artist.UserArtistReply user_artist
        {
            get { return _user_artist; }
            set { _user_artist = value; }
        }

        private artist.ListArtistReply _list_artist = null;
        [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name = @"list_artist", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(null)]
        public artist.ListArtistReply list_artist
        {
            get { return _list_artist; }
            set { _list_artist = value; }
        }

    }

}
// Generated from: artist.proto
// Note: requires additional types generated from: comm.proto
namespace artist
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"CreateArtistRequest")]
    public partial class CreateArtistRequest : global::ProtoBuf.IExtensible
    {
        public CreateArtistRequest() { }

        private string _artist_name;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string artist_name
        {
            get { return _artist_name; }
            set { _artist_name = value; }
        }
        private string _description;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _avatar_url = "";
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"avatar_url", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string avatar_url
        {
            get { return _avatar_url; }
            set { _avatar_url = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"CreateArtistReply")]
    public partial class CreateArtistReply : global::ProtoBuf.IExtensible
    {
        public CreateArtistReply() { }

        private ulong _artist_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong artist_id
        {
            get { return _artist_id; }
            set { _artist_id = value; }
        }
        [global::ProtoBuf.ProtoContract(Name = @"Type")]
        public enum Type
        {

            [global::ProtoBuf.ProtoEnum(Name = @"NAME_ERROR", Value = 4)]
            NAME_ERROR = 4
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ArtistInfoRequest")]
    public partial class ArtistInfoRequest : global::ProtoBuf.IExtensible
    {
        public ArtistInfoRequest() { }

        private ulong _artist_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong artist_id
        {
            get { return _artist_id; }
            set { _artist_id = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ArtistInfoReply")]
    public partial class ArtistInfoReply : global::ProtoBuf.IExtensible
    {
        public ArtistInfoReply() { }

        private ulong _artist_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong artist_id
        {
            get { return _artist_id; }
            set { _artist_id = value; }
        }
        private string _artist_name;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"artist_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string artist_name
        {
            get { return _artist_name; }
            set { _artist_name = value; }
        }
        private string _description;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        private uint _motion_count;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"motion_count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint motion_count
        {
            get { return _motion_count; }
            set { _motion_count = value; }
        }
        private uint _follower_count;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"follower_count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint follower_count
        {
            get { return _follower_count; }
            set { _follower_count = value; }
        }
        private uint _avg_rating;
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"avg_rating", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint avg_rating
        {
            get { return _avg_rating; }
            set { _avg_rating = value; }
        }

        private string _avatar_url = "";
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"avatar_url", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string avatar_url
        {
            get { return _avatar_url; }
            set { _avatar_url = value; }
        }
        [global::ProtoBuf.ProtoContract(Name = @"Type")]
        public enum Type
        {

            [global::ProtoBuf.ProtoEnum(Name = @"NO_ARTIST", Value = 4)]
            NO_ARTIST = 4
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"UpdateArtistRequest")]
    public partial class UpdateArtistRequest : global::ProtoBuf.IExtensible
    {
        public UpdateArtistRequest() { }

        private ulong _artist_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong artist_id
        {
            get { return _artist_id; }
            set { _artist_id = value; }
        }

        private string _artist_name = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"artist_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string artist_name
        {
            get { return _artist_name; }
            set { _artist_name = value; }
        }

        private string _description = "";
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _avatar_url = "";
        [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"avatar_url", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string avatar_url
        {
            get { return _avatar_url; }
            set { _avatar_url = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"UpdateArtistReply")]
    public partial class UpdateArtistReply : global::ProtoBuf.IExtensible
    {
        public UpdateArtistReply() { }

        private ulong _artist_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong artist_id
        {
            get { return _artist_id; }
            set { _artist_id = value; }
        }
        [global::ProtoBuf.ProtoContract(Name = @"Type")]
        public enum Type
        {

            [global::ProtoBuf.ProtoEnum(Name = @"NO_ARTIST", Value = 4)]
            NO_ARTIST = 4,

            [global::ProtoBuf.ProtoEnum(Name = @"NAME_ERROR", Value = 5)]
            NAME_ERROR = 5
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"UserArtistRequest")]
    public partial class UserArtistRequest : global::ProtoBuf.IExtensible
    {
        public UserArtistRequest() { }

        private ulong _user_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"user_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong user_id
        {
            get { return _user_id; }
            set { _user_id = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"UserArtistReply")]
    public partial class UserArtistReply : global::ProtoBuf.IExtensible
    {
        public UserArtistReply() { }

        private ulong _user_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"user_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong user_id
        {
            get { return _user_id; }
            set { _user_id = value; }
        }
        private readonly global::System.Collections.Generic.List<artist.ArtistShortInfo> _artist_short_info = new global::System.Collections.Generic.List<artist.ArtistShortInfo>();
        [global::ProtoBuf.ProtoMember(2, Name = @"artist_short_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public global::System.Collections.Generic.List<artist.ArtistShortInfo> artist_short_info
        {
            get { return _artist_short_info; }
        }

        [global::ProtoBuf.ProtoContract(Name = @"Type")]
        public enum Type
        {

            [global::ProtoBuf.ProtoEnum(Name = @"NO_USER", Value = 4)]
            NO_USER = 4
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ListArtistRequest")]
    public partial class ListArtistRequest : global::ProtoBuf.IExtensible
    {
        public ListArtistRequest() { }

        private uint _start;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"start", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint start
        {
            get { return _start; }
            set { _start = value; }
        }
        private uint _end;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"end", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint end
        {
            get { return _end; }
            set { _end = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ListArtistReply")]
    public partial class ListArtistReply : global::ProtoBuf.IExtensible
    {
        public ListArtistReply() { }

        private readonly global::System.Collections.Generic.List<artist.ArtistShortInfo> _artist_short_info = new global::System.Collections.Generic.List<artist.ArtistShortInfo>();
        [global::ProtoBuf.ProtoMember(1, Name = @"artist_short_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public global::System.Collections.Generic.List<artist.ArtistShortInfo> artist_short_info
        {
            get { return _artist_short_info; }
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"ArtistShortInfo")]
    public partial class ArtistShortInfo : global::ProtoBuf.IExtensible
    {
        public ArtistShortInfo() { }

        private ulong _artist_id;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"artist_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public ulong artist_id
        {
            get { return _artist_id; }
            set { _artist_id = value; }
        }
        private string _artist_name;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"artist_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string artist_name
        {
            get { return _artist_name; }
            set { _artist_name = value; }
        }
        private uint _avg_rating;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"avg_rating", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint avg_rating
        {
            get { return _avg_rating; }
            set { _avg_rating = value; }
        }
        private uint _motions_count;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"motions_count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public uint motions_count
        {
            get { return _motions_count; }
            set { _motions_count = value; }
        }

        private string _avatar_url = "";
        [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"avatar_url", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string avatar_url
        {
            get { return _avatar_url; }
            set { _avatar_url = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

}