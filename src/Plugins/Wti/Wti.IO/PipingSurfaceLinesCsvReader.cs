using System;

namespace Wti.IO
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv), containing
    /// data specifying surfacelines. 
    /// Expects data to be specified in the following format:
    /// <para><c>{ID};X1;Y1;Z1...;(Xn;Yn;Zn)</c></para>
    /// Where {ID} has to be a particular accepted text, and n triplets of doubles form the
    /// 3D coordinates defining the geometric shape of the surfaceline..
    /// </summary>
    public class PipingSurfaceLinesCsvReader
    {
        private readonly string[] acceptableIdNames = { "Profielnaam, LocationID" };
    }
}