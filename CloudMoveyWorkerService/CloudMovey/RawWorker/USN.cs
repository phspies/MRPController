using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PInvoke;

namespace ConsoleUSNJournalProject
{
    [Serializable]
    public class FileNameAndParentFrn : IEquatable<FileNameAndParentFrn>
    {
        #region Properties

        private uint _volSer;
        public uint VolSer
        {
            get { return _volSer; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private UInt64 _frn;
        public UInt64 Frn
        {
            get { return _frn; }
            set { _frn = value; }
        }

        private UInt64 _parentFrn;
        public UInt64 ParentFrn
        {
            get { return _parentFrn; }
            set { _parentFrn = value; }
        }

        private byte[] _md5;
        public byte[] MD5
        {
            get { return _md5; }
            set { _md5 = value; }
        }

        #endregion

        #region Constructor(s)

        public FileNameAndParentFrn(uint volser, string name, UInt64 frn, UInt64 parentFrn)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _name = name;
            }
            else
            {
                throw new ArgumentException("Invalid argument: null or Length = zero", "name");
            }
            _volSer = volser;
            _parentFrn = parentFrn;
            _frn = frn;
            _md5 = null;
        }   // end FileNameAndParentFrn() closing bracket

        public FileNameAndParentFrn(uint volser, ulong frn)
        {
            _volSer = volser;
            _frn = frn;
        }

        public FileNameAndParentFrn(uint volser, Win32Api.USN_RECORD usn)
        {
            if (!string.IsNullOrEmpty(usn.FileName))
            {
                _name = usn.FileName;
            }
            else
            {
                throw new ArgumentException("Invalid argument: null or Length = zero", "name");
            }
            _volSer = volser;
            _parentFrn = usn.ParentFileReferenceNumber;
            _frn = usn.FileReferenceNumber;
            _md5 = null;
        }   // end FileNameAndParentFrn() closing bracket

        #endregion

        #region IEquatable<FileNameAndParentFrn> Members

        public bool Equals(FileNameAndParentFrn other)
        {
            bool retval = false;
            if ((object)other != null)
            {
                if (_frn == other._frn && _volSer == other.VolSer)
                    retval = true;
            }
            return retval;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);

            if (!(obj is FileNameAndParentFrn))
            {
                throw new InvalidCastException("Argument is not a 'FileNameAndParentFrn' type object");
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this._frn.GetHashCode();
        }

        public static bool operator ==(FileNameAndParentFrn fnapfrn1, FileNameAndParentFrn fnapfrn2)
        {
            return fnapfrn1.Equals(fnapfrn2);
        }

        public static bool operator !=(FileNameAndParentFrn fnapfrn1, FileNameAndParentFrn fnapfrn2)
        {
            return (!fnapfrn1.Equals(fnapfrn2));
        }

        #endregion
    }
}