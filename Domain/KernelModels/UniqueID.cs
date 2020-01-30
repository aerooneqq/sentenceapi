using System;

namespace DataAccessLayer.KernelModels
{
    public class UniqueID
    {
        public long ID { get; }

        public UniqueID() { }
        public UniqueID(long id) => ID = id;
        public UniqueID(UniqueID uniqueID) => ID = uniqueID.ID;
        

        #region Overriden methods
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is UniqueID))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            UniqueID uniqueID = obj as UniqueID;

            if (ID == uniqueID.ID)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode() => ID.GetHashCode();

        public override string ToString() => ID.ToString();
        #endregion

        #region Operators
        public static bool operator == (UniqueID firstID, UniqueID secondID)
        {
            return firstID.Equals(secondID);
        }

        public static bool operator != (UniqueID firstID, UniqueID secondID)
        {
            return !firstID.Equals(secondID);
        }
        #endregion

    }
}