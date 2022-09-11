﻿namespace AndriiCo.TaxCalculatorLib.Entities
{
    /// <summary>
    /// Class to represent tax payer location
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Location name
        /// </summary>
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        #region Equals override generated by Resharper
        protected bool Equals(Location other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Location) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
        #endregion
    }
}