using System;
using System.Diagnostics;
using System.IO;

namespace Core.GIS.NetTopologySuite.IO.GeoTools.Dbase
{
    /// <summary>
    /// Class for holding the information assicated with a dbase header.
    /// </summary>
    public class DbaseFileHeader
    {
        // Constant for the size of a record
        private readonly int FileDescriptorSize = 32;

        // type of the file, must be 03h
        private int _fileType = 0x03;

        // Date the file was last updated.

        // Number of records in the datafile

        // Length of the header structure

        // Length of the records

        // Number of fields in the record.

        // collection of header records.

        /// <summary>
        /// Initializes a new instance of the DbaseFileHeader class.
        /// </summary>
        public DbaseFileHeader()
        {
            NumRecords = 0;
        }

        /// <summary>
        /// Return the date this file was last updated.
        /// </summary>
        /// <returns></returns>
        public DateTime LastUpdateDate { get; private set; }

        /// <summary>
        /// Return the number of fields in the records.
        /// </summary>
        /// <returns></returns>
        public int NumFields { get; set; }

        /// <summary>
        /// Return the number of records in the file.
        /// </summary>
        /// <returns></returns>
        public int NumRecords { get; set; }

        /// <summary>
        /// Return the length of the records in bytes.
        /// </summary>
        /// <returns></returns>
        public int RecordLength { get; private set; }

        /// <summary>
        /// Return the length of the header.
        /// </summary>
        /// <returns></returns>
        public int HeaderLength { get; private set; }

        /// <summary>
        /// Returns the fields in the dbase file.
        /// </summary>
        public DbaseFieldDescriptor[] Fields { get; private set; }

        /// <summary>
        ///  Add a column to this DbaseFileHeader.
        /// </summary>
        /// <param name="fieldName">The name of the field to add.</param>
        /// <param name="fieldType">The type is one of (C N L or D) character, number, logical(true/false), or date.</param>
        /// <param name="fieldLength"> The Field length is the total length in bytes reserved for this column.</param>
        /// <param name="decimalCount">The decimal count only applies to numbers(N), and floating point values (F), and refers to the number of characters to reserve after the decimal point.</param>
        public void AddColumn(string fieldName, char fieldType, int fieldLength, int decimalCount)
        {
            if (fieldLength <= 0)
            {
                fieldLength = 1;
            }
            if (Fields == null)
            {
                Fields = new DbaseFieldDescriptor[0];
            }
            int tempLength = 1; // the length is used for the offset, and there is a * for deleted as the first byte
            DbaseFieldDescriptor[] tempFieldDescriptors = new DbaseFieldDescriptor[Fields.Length + 1];
            for (int i = 0; i < Fields.Length; i++)
            {
                Fields[i].DataAddress = tempLength;
                tempLength = tempLength + Fields[i].Length;
                tempFieldDescriptors[i] = Fields[i];
            }
            tempFieldDescriptors[Fields.Length] = new DbaseFieldDescriptor();
            tempFieldDescriptors[Fields.Length].Length = fieldLength;
            tempFieldDescriptors[Fields.Length].DecimalCount = decimalCount;
            tempFieldDescriptors[Fields.Length].DataAddress = tempLength;

            // set the field name
            string tempFieldName = fieldName;
            if (tempFieldName == null)
            {
                tempFieldName = "NoName";
            }
            if (tempFieldName.Length > 11)
            {
                tempFieldName = tempFieldName.Substring(0, 11);
                Trace.Write("FieldName " + fieldName + " is longer than 11 characters, truncating to " + tempFieldName);
            }
            tempFieldDescriptors[Fields.Length].Name = tempFieldName;

            // the field type
            if ((fieldType == 'C') || (fieldType == 'c'))
            {
                tempFieldDescriptors[Fields.Length].DbaseType = 'C';
                if (fieldLength > 254)
                {
                    Trace.WriteLine("Field Length for " + fieldName + " set to " + fieldLength + " Which is longer than 254, not consistent with dbase III");
                }
            }
            else if ((fieldType == 'S') || (fieldType == 's'))
            {
                tempFieldDescriptors[Fields.Length].DbaseType = 'C';
                Trace.WriteLine("Field type for " + fieldName + " set to S which is flat out wrong people!, I am setting this to C, in the hopes you meant character.");
                if (fieldLength > 254)
                {
                    Trace.WriteLine("Field Length for " + fieldName + " set to " + fieldLength + " Which is longer than 254, not consistent with dbase III");
                }
                tempFieldDescriptors[Fields.Length].Length = 8;
            }
            else if ((fieldType == 'D') || (fieldType == 'd'))
            {
                tempFieldDescriptors[Fields.Length].DbaseType = 'D';
                if (fieldLength != 8)
                {
                    Trace.WriteLine("Field Length for " + fieldName + " set to " + fieldLength + " Setting to 8 digets YYYYMMDD");
                }
                tempFieldDescriptors[Fields.Length].Length = 8;
            }
            else if ((fieldType == 'F') || (fieldType == 'f'))
            {
                tempFieldDescriptors[Fields.Length].DbaseType = 'F';
                if (fieldLength > 20)
                {
                    Trace.WriteLine("Field Length for " + fieldName + " set to " + fieldLength + " Preserving length, but should be set to Max of 20 not valid for dbase IV, and UP specification, not present in dbaseIII.");
                }
            }
            else if ((fieldType == 'N') || (fieldType == 'n'))
            {
                tempFieldDescriptors[Fields.Length].DbaseType = 'N';
                if (fieldLength > 18)
                {
                    Trace.WriteLine("Field Length for " + fieldName + " set to " + fieldLength + " Preserving length, but should be set to Max of 18 for dbase III specification.");
                }
                if (decimalCount < 0)
                {
                    Trace.WriteLine("Field Decimal Position for " + fieldName + " set to " + decimalCount + " Setting to 0 no decimal data will be saved.");
                    tempFieldDescriptors[Fields.Length].DecimalCount = 0;
                }
                if (decimalCount > fieldLength - 1)
                {
                    Trace.WriteLine("Field Decimal Position for " + fieldName + " set to " + decimalCount + " Setting to " + (fieldLength - 1) + " no non decimal data will be saved.");
                    tempFieldDescriptors[Fields.Length].DecimalCount = fieldLength - 1;
                }
            }
            else if ((fieldType == 'L') || (fieldType == 'l'))
            {
                tempFieldDescriptors[Fields.Length].DbaseType = 'L';
                if (fieldLength != 1)
                {
                    Trace.WriteLine("Field Length for " + fieldName + " set to " + fieldLength + " Setting to length of 1 for logical fields.");
                }
                tempFieldDescriptors[Fields.Length].Length = 1;
            }
            else
            {
                throw new NotSupportedException("Unsupported field type " + fieldType + " For column " + fieldName);
            }
            // the length of a record
            tempLength = tempLength + tempFieldDescriptors[Fields.Length].Length;

            // set the new fields.
            Fields = tempFieldDescriptors;
            HeaderLength = 33 + 32*Fields.Length;
            NumFields = Fields.Length;
            RecordLength = tempLength;
        }

        /// <summary>
        /// Remove a column from this DbaseFileHeader.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>return index of the removed column, -1 if no found.</returns>
        public int RemoveColumn(string fieldName)
        {
            int retCol = -1;
            int tempLength = 1;
            DbaseFieldDescriptor[] tempFieldDescriptors =
                new DbaseFieldDescriptor[Fields.Length - 1];
            for (int i = 0, j = 0; i < Fields.Length; i++)
            {
                if (fieldName.ToLower() != (Fields[i].Name.Trim().ToLower()))
                {
                    // if this is the last field and we still haven't found the
                    // named field
                    if (i == j && i == Fields.Length - 1)
                    {
                        return retCol;
                    }
                    tempFieldDescriptors[j] = Fields[i];
                    tempFieldDescriptors[j].DataAddress = tempLength;
                    tempLength += tempFieldDescriptors[j].Length;
                    // only increment j on non-matching fields
                    j++;
                }
                else
                {
                    retCol = i;
                }
            }

            // set the new fields.
            Fields = tempFieldDescriptors;
            HeaderLength = 33 + 32*Fields.Length;
            NumFields = Fields.Length;
            RecordLength = tempLength;

            return retCol;
        }

        /// <summary>
        /// Read the header data from the DBF file.
        /// </summary>
        /// <param name="reader">BinaryReader containing the header.</param>
        public void ReadHeader(BinaryReader reader)
        {
            // type of reader.
            _fileType = reader.ReadByte();
            if (_fileType != 0x03)
            {
                throw new NotSupportedException("Unsupported DBF reader Type " + _fileType);
            }

            // parse the update date information.
            int year = (int) reader.ReadByte();
            int month = (int) reader.ReadByte();
            int day = (int) reader.ReadByte();
            LastUpdateDate = new DateTime(year + 1900, month, day);

            // read the number of records.
            NumRecords = reader.ReadInt32();

            // read the length of the header structure.
            HeaderLength = reader.ReadInt16();

            // read the length of a record
            RecordLength = reader.ReadInt16();

            // skip the reserved bytes in the header.
            //in.skipBytes(20);
            reader.ReadBytes(20);
            // calculate the number of Fields in the header
            NumFields = (HeaderLength - FileDescriptorSize - 1)/FileDescriptorSize;

            // read all of the header records
            Fields = new DbaseFieldDescriptor[NumFields];
            for (int i = 0; i < NumFields; i++)
            {
                Fields[i] = new DbaseFieldDescriptor();

                // read the field name				
                char[] buffer = new char[11];
                buffer = reader.ReadChars(11);
                string name = new string(buffer);
                int nullPoint = name.IndexOf((char) 0);
                if (nullPoint != -1)
                {
                    name = name.Substring(0, nullPoint);
                }
                Fields[i].Name = name;

                // read the field type
                Fields[i].DbaseType = (char) reader.ReadByte();

                // read the field data address, offset from the start of the record.
                Fields[i].DataAddress = reader.ReadInt32();

                // read the field length in bytes
                int tempLength = (int) reader.ReadByte();
                if (tempLength < 0)
                {
                    tempLength = tempLength + 256;
                }
                Fields[i].Length = tempLength;

                // read the field decimal count in bytes
                Fields[i].DecimalCount = (int) reader.ReadByte();

                // read the reserved bytes.
                //reader.skipBytes(14);
                reader.ReadBytes(14);
            }

            // Last byte is a marker for the end of the field definitions.
            reader.ReadBytes(1);
        }

        /// <summary>
        /// Write the header data to the DBF file.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteHeader(BinaryWriter writer)
        {
            // write the output file type.
            writer.Write((byte) _fileType);

            writer.Write((byte) (LastUpdateDate.Year - 1900));
            writer.Write((byte) LastUpdateDate.Month);
            writer.Write((byte) LastUpdateDate.Day);

            // write the number of records in the datafile.
            writer.Write(NumRecords);

            // write the length of the header structure.
            writer.Write((short) HeaderLength);

            // write the length of a record
            writer.Write((short) RecordLength);

            // write the reserved bytes in the header
            for (int i = 0; i < 20; i++)
            {
                writer.Write((byte) 0);
            }

            // write all of the header records
            int tempOffset = 0;
            for (int i = 0; i < Fields.Length; i++)
            {
                // write the field name
                for (int j = 0; j < 11; j++)
                {
                    if (Fields[i].Name.Length > j)
                    {
                        writer.Write((byte) Fields[i].Name[j]);
                    }
                    else
                    {
                        writer.Write((byte) 0);
                    }
                }

                // write the field type
                writer.Write((char) Fields[i].DbaseType);

                // write the field data address, offset from the start of the record.
                writer.Write(0);
                tempOffset += Fields[i].Length;

                // write the length of the field.
                writer.Write((byte) Fields[i].Length);

                // write the decimal count.
                writer.Write((byte) Fields[i].DecimalCount);

                // write the reserved bytes.
                for (int j = 0; j < 14; j++)
                {
                    writer.Write((byte) 0);
                }
            }

            // write the end of the field definitions marker
            writer.Write((byte) 0x0D);
        }

        /// <summary>
        /// Set the number of records in the file
        /// </summary>
        /// <param name="inNumRecords"></param>
        protected void SetNumRecords(int inNumRecords)
        {
            NumRecords = inNumRecords;
        }
    }
}