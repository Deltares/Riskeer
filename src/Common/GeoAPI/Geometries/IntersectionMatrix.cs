using System;
using System.Text;

namespace GeoAPI.Geometries
{
    /// <summary>  
    /// A Dimensionally Extended Nine-Intersection Model (DE-9IM) matrix. This class
    /// can used to represent both computed DE-9IM's (like 212FF1FF2) as well as
    /// patterns for matching them (like T*T******). 
    /// <para>
    /// Methods are provided to:
    /// - Set and query the elements of the matrix in a convenient fashion.
    /// - Convert to and from the standard string representation (specified in SFS Section 2.1.13.2).
    /// - Test to see if a matrix matches a given pattern string.
    /// </para>
    /// <remarks>
    /// For a description of the DE-9IM, see the 
    /// <see href="http://www.opengis.org/techno/specs.htm">OpenGIS Simple Features Specification for SQL.</see>
    /// </remarks>
    /// </summary>       
    public class IntersectionMatrix
    {
        /// <summary>  
        /// Internal representation of this <see cref="IntersectionMatrix" />.
        /// </summary>
        private Dimensions[,] matrix;

        /// <summary>  
        /// Creates an <see cref="IntersectionMatrix" /> with <c>null</c> location values.
        /// </summary>
        public IntersectionMatrix()
        {
            matrix = new Dimensions[3, 3];
            SetAll(Dimensions.False);
        }

        /// <summary>
        /// Creates an <see cref="IntersectionMatrix" /> with the given dimension
        /// symbols.
        /// </summary>
        /// <param name="elements">A string of nine dimension symbols in row major order.</param>
        public IntersectionMatrix(string elements) : this()
        {
            Set(elements);
        }

        /// <summary> 
        /// Creates an <see cref="IntersectionMatrix" /> with the same elements as
        /// <c>other</c>.
        /// </summary>
        /// <param name="other">An <see cref="IntersectionMatrix" /> to copy.</param>         
        public IntersectionMatrix(IntersectionMatrix other) : this()
        {
            matrix[(int) Locations.Interior, (int)Locations.Interior] = other.matrix[(int) Locations.Interior, (int) Locations.Interior];
            matrix[(int) Locations.Interior, (int)Locations.Boundary] = other.matrix[(int) Locations.Interior, (int) Locations.Boundary];
            matrix[(int) Locations.Interior, (int)Locations.Exterior] = other.matrix[(int) Locations.Interior, (int) Locations.Exterior];
            matrix[(int) Locations.Boundary, (int)Locations.Interior] = other.matrix[(int) Locations.Boundary, (int) Locations.Interior];
            matrix[(int) Locations.Boundary, (int)Locations.Boundary] = other.matrix[(int) Locations.Boundary, (int) Locations.Boundary];
            matrix[(int) Locations.Boundary, (int)Locations.Exterior] = other.matrix[(int) Locations.Boundary, (int) Locations.Exterior];
            matrix[(int) Locations.Exterior, (int)Locations.Interior] = other.matrix[(int) Locations.Exterior, (int) Locations.Interior];
            matrix[(int) Locations.Exterior, (int)Locations.Boundary] = other.matrix[(int) Locations.Exterior, (int) Locations.Boundary];
            matrix[(int) Locations.Exterior, (int)Locations.Exterior] = other.matrix[(int) Locations.Exterior, (int) Locations.Exterior];
        }

        /// <summary> 
        /// Adds one matrix to another.
        /// Addition is defined by taking the maximum dimension value of each position
        /// in the summand matrices.
        /// </summary>
        /// <param name="im">The matrix to add.</param>        
        public void Add(IntersectionMatrix im)
        {
            for (int i = 0; i < 3; i++)            
                for (int j = 0; j < 3; j++)
                    SetAtLeast((Locations) i, (Locations) j, im.Get((Locations) i, (Locations) j));            
        }

        /// <summary>  
        /// Returns true if the dimension value satisfies the dimension symbol.
        /// </summary>
        /// <param name="actualDimensionValue">
        /// A number that can be stored in the <see cref="IntersectionMatrix" />. 
        /// Possible values are <c>True, False, Dontcare, 0, 1, 2</c>.
        /// </param>
        /// <param name="requiredDimensionSymbol">
        /// A character used in the string
        /// representation of an <see cref="IntersectionMatrix" />. 
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the dimension symbol encompasses the dimension value.        
        /// </returns>        
        public static bool Matches(Dimensions actualDimensionValue, char requiredDimensionSymbol)
        {                      
            if (requiredDimensionSymbol == '*')
                return true;            
            if (requiredDimensionSymbol == 'T' && (actualDimensionValue >= Dimensions.Point || actualDimensionValue == Dimensions.True))
                return true;            
            if (requiredDimensionSymbol == 'F' && actualDimensionValue == Dimensions.False)
                return true;            
            if (requiredDimensionSymbol == '0' && actualDimensionValue == Dimensions.Point)
                return true;            
            if (requiredDimensionSymbol == '1' && actualDimensionValue == Dimensions.Curve)
                return true;
            if (requiredDimensionSymbol == '2' && actualDimensionValue == Dimensions.Surface)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if each of the actual dimension symbols satisfies the
        /// corresponding required dimension symbol.
        /// </summary>
        /// <param name="actualDimensionSymbols">
        /// Nine dimension symbols to validate.
        /// Possible values are <c>T, F, * , 0, 1, 2</c>.
        /// </param>
        /// <param name="requiredDimensionSymbols">
        /// Nine dimension symbols to validate
        /// against. Possible values are <c>T, F, * , 0, 1, 2</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if each of the required dimension
        /// symbols encompass the corresponding actual dimension symbol.
        /// </returns>
        public static bool Matches(string actualDimensionSymbols, string requiredDimensionSymbols)
        {
            IntersectionMatrix m = new IntersectionMatrix(actualDimensionSymbols);
            return m.Matches(requiredDimensionSymbols);
        }

        /// <summary>
        /// Changes the value of one of this <see cref="IntersectionMatrix" /> elements.
        /// </summary>
        /// <param name="row">
        /// The row of this <see cref="IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the first <see cref="IGeometry"/>
        /// </param>
        /// <param name="column">
        /// The column of this <see cref="IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the second <see cref="IGeometry"/>
        /// </param>
        /// <param name="dimensionValue">The new value of the element</param>        
        public void Set(Locations row, Locations column, Dimensions dimensionValue)
        {
            matrix[(int) row, (int) column] = dimensionValue;
        }

        /// <summary>
        /// Changes the elements of this <see cref="IntersectionMatrix" /> to the
        /// dimension symbols in <c>dimensionSymbols</c>.
        /// </summary>
        /// <param name="dimensionSymbols">
        /// Nine dimension symbols to which to set this <see cref="IntersectionMatrix" />
        /// s elements. Possible values are <c>{T, F, * , 0, 1, 2}</c>
        /// </param>
        public void Set(string dimensionSymbols)
        {
            for (int i = 0; i < dimensionSymbols.Length; i++)
            {
                int row = i / 3;
                int col = i % 3;
                matrix[row, col] = Dimension.ToDimensionValue(dimensionSymbols[i]);
            }
        }

        /// <summary>
        /// Changes the specified element to <c>minimumDimensionValue</c> if the element is less.
        /// </summary>
        /// <param name="row">
        /// The row of this <see cref="IntersectionMatrix" />, 
        /// indicating the interior, boundary or exterior of the first <see cref="IGeometry"/>.
        /// </param>
        /// <param name="column">
        /// The column of this <see cref="IntersectionMatrix" />, 
        /// indicating the interior, boundary or exterior of the second <see cref="IGeometry"/>.
        /// </param>
        /// <param name="minimumDimensionValue">
        /// The dimension value with which to compare the
        /// element. The order of dimension values from least to greatest is
        /// <c>True, False, Dontcare, 0, 1, 2</c>.
        /// </param>
        public void SetAtLeast(Locations row, Locations column, Dimensions minimumDimensionValue)
        {
            if (matrix[(int) row, (int) column] < minimumDimensionValue)
                matrix[(int) row, (int) column] = minimumDimensionValue;            
        }
        
        /// <summary>
        /// If row >= 0 and column >= 0, changes the specified element to <c>minimumDimensionValue</c>
        /// if the element is less. Does nothing if row is smaller to 0 or column is smaller to 0.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="minimumDimensionValue"></param>
        public void SetAtLeastIfValid(Locations row, Locations column, Dimensions minimumDimensionValue)
        {
            if (row >= Locations.Interior && column >= Locations.Interior)
                SetAtLeast(row, column, minimumDimensionValue);            
        }
  
        /// <summary>
        /// For each element in this <see cref="IntersectionMatrix" />, changes the
        /// element to the corresponding minimum dimension symbol if the element is
        /// less.
        /// </summary>
        /// <param name="minimumDimensionSymbols"> 
        /// Nine dimension symbols with which to
        /// compare the elements of this <see cref="IntersectionMatrix" />. The
        /// order of dimension values from least to greatest is <c>Dontcare, True, False, 0, 1, 2</c>.
        /// </param>
        public void SetAtLeast(string minimumDimensionSymbols)
        {
            for (int i = 0; i < minimumDimensionSymbols.Length; i++)
            {
                int row = i / 3;
                int col = i % 3;
                SetAtLeast((Locations) row, (Locations) col, Dimension.ToDimensionValue(minimumDimensionSymbols[i]));
            }
        }

        /// <summary>  
        /// Changes the elements of this <see cref="IntersectionMatrix" /> to <c>dimensionValue</c>.
        /// </summary>
        /// <param name="dimensionValue">
        /// The dimension value to which to set this <see cref="IntersectionMatrix" />
        /// s elements. Possible values <c>True, False, Dontcare, 0, 1, 2}</c>.
        /// </param>         
        public void SetAll(Dimensions dimensionValue)
        {
            for (int ai = 0; ai < 3; ai++)
                for (int bi = 0; bi < 3; bi++)                
                    matrix[ai, bi] = dimensionValue;            
        }

        /// <summary>
        /// Returns the value of one of this <see cref="IntersectionMatrix" />s
        /// elements.
        /// </summary>
        /// <param name="row">
        /// The row of this <see cref="IntersectionMatrix" />, indicating
        /// the interior, boundary or exterior of the first <see cref="IGeometry"/>.
        /// </param>
        /// <param name="column">
        /// The column of this <see cref="IntersectionMatrix" />,
        /// indicating the interior, boundary or exterior of the second <see cref="IGeometry"/>.
        /// </param>
        /// <returns>The dimension value at the given matrix position.</returns>
        public Dimensions Get(Locations row, Locations column)
        {
            return matrix[(int) row, (int) column];
        }

        /// <summary>
        /// See methods Get(int, int) and Set(int, int, int value)
        /// </summary>         
        public Dimensions this[Locations row, Locations column]
        {
            get
            {
                return Get(row, column);
            }
            set
            {
                Set(row, column, value);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is FF*FF****.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the two <see cref="IGeometry"/>'s related by
        /// this <see cref="IntersectionMatrix" /> are disjoint.
        /// </returns>
        public bool IsDisjoint()
        {
            return
                matrix[(int) Locations.Interior, (int) Locations.Interior] == Dimensions.False &&
                matrix[(int) Locations.Interior, (int) Locations.Boundary] == Dimensions.False &&
                matrix[(int) Locations.Boundary, (int) Locations.Interior] == Dimensions.False &&
                matrix[(int) Locations.Boundary, (int) Locations.Boundary] == Dimensions.False;
        }

        /// <summary>
        /// Returns <c>true</c> if <c>isDisjoint</c> returns false.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the two <see cref="IGeometry"/>'s related by
        /// this <see cref="IntersectionMatrix" /> intersect.
        /// </returns>
        public bool IsIntersects()
        {
            return !IsDisjoint();
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is
        /// FT*******, F**T***** or F***T****.
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="IGeometry"/>.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="IGeometry"/>.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="IGeometry"/>
        /// s related by this <see cref="IntersectionMatrix" /> touch; Returns false
        /// if both <see cref="IGeometry"/>s are points.
        /// </returns>
        public bool IsTouches(Dimensions dimensionOfGeometryA, Dimensions dimensionOfGeometryB)
        {
            if (dimensionOfGeometryA > dimensionOfGeometryB)
                //no need to get transpose because pattern matrix is symmetrical
                return IsTouches(dimensionOfGeometryB, dimensionOfGeometryA);            

            if ((dimensionOfGeometryA == Dimensions.Surface && dimensionOfGeometryB == Dimensions.Surface)   ||
                (dimensionOfGeometryA == Dimensions.Curve   && dimensionOfGeometryB == Dimensions.Curve)     ||
                (dimensionOfGeometryA == Dimensions.Curve   && dimensionOfGeometryB == Dimensions.Surface)   ||
                (dimensionOfGeometryA == Dimensions.Point   && dimensionOfGeometryB == Dimensions.Surface)   ||
                (dimensionOfGeometryA == Dimensions.Point   && dimensionOfGeometryB == Dimensions.Curve))            
                return matrix[(int) Locations.Interior, (int) Locations.Interior] == Dimensions.False &&
                        (Matches(matrix[(int) Locations.Interior, (int) Locations.Boundary], 'T') ||
                         Matches(matrix[(int) Locations.Boundary, (int) Locations.Interior], 'T') ||  
                         Matches(matrix[(int) Locations.Boundary, (int) Locations.Boundary], 'T'));
            
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is
        ///  T*T****** (for a point and a curve, a point and an area or a line
        /// and an area) 0******** (for two curves).
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="IGeometry"/>.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="IGeometry"/>.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="IGeometry"/>
        /// s related by this <see cref="IntersectionMatrix" /> cross. For this
        /// function to return <c>true</c>, the <see cref="IGeometry"/>s must
        /// be a point and a curve; a point and a surface; two curves; or a curve
        /// and a surface.
        /// </returns>
        public bool IsCrosses(Dimensions dimensionOfGeometryA, Dimensions dimensionOfGeometryB)
        {
            if ((dimensionOfGeometryA == Dimensions.Point   && dimensionOfGeometryB == Dimensions.Curve)   ||
                (dimensionOfGeometryA == Dimensions.Point   && dimensionOfGeometryB == Dimensions.Surface) ||
                (dimensionOfGeometryA == Dimensions.Curve   && dimensionOfGeometryB == Dimensions.Surface))            
                return Matches(matrix[(int) Locations.Interior, (int) Locations.Interior], 'T') &&
                       Matches(matrix[(int) Locations.Interior, (int) Locations.Exterior], 'T');
            
            if ((dimensionOfGeometryA == Dimensions.Curve   && dimensionOfGeometryB == Dimensions.Point)   ||
                (dimensionOfGeometryA == Dimensions.Surface && dimensionOfGeometryB == Dimensions.Point)   ||
                (dimensionOfGeometryA == Dimensions.Surface && dimensionOfGeometryB == Dimensions.Curve))            
                return Matches(matrix[(int) Locations.Interior, (int) Locations.Interior], 'T') &&
                       Matches(matrix[(int) Locations.Exterior, (int) Locations.Interior], 'T');
            
            if (dimensionOfGeometryA == Dimensions.Curve && dimensionOfGeometryB == Dimensions.Curve)            
                return matrix[(int) Locations.Interior, (int) Locations.Interior] == 0;            
            
            return false;
        }

        /// <summary>  
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is
        /// T*F**F***.
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="IGeometry"/> is within the second.</returns>
        public bool IsWithin()
        {
            return Matches( matrix[(int) Locations.Interior, (int) Locations.Interior], 'T') &&
                            matrix[(int) Locations.Interior, (int) Locations.Exterior] == Dimensions.False &&
                            matrix[(int) Locations.Boundary, (int) Locations.Exterior] == Dimensions.False;
        }

        /// <summary> 
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is
        /// T*****FF*.
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="IGeometry"/> contains the second.</returns>
        public bool IsContains()
        {
            return Matches( matrix[(int) Locations.Interior, (int) Locations.Interior], 'T') &&
                            matrix[(int) Locations.Exterior, (int) Locations.Interior] == Dimensions.False &&
                            matrix[(int) Locations.Exterior, (int) Locations.Boundary] == Dimensions.False;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is <c>T*****FF*</c>
        /// or <c>*T****FF*</c> or <c>***T**FF*</c> or <c>****T*FF*</c>.
        /// </summary>
        /// <returns><c>true</c> if the first <see cref="IGeometry"/> covers the second</returns>
        public bool IsCovers()
        {
            bool hasPointInCommon =     Matches(matrix[(int) Locations.Interior, (int) Locations.Interior], 'T')
                                    ||  Matches(matrix[(int) Locations.Interior, (int) Locations.Boundary], 'T')
                                    ||  Matches(matrix[(int) Locations.Boundary, (int) Locations.Interior], 'T')
                                    ||  Matches(matrix[(int) Locations.Boundary, (int) Locations.Boundary], 'T');

            return hasPointInCommon &&
                    matrix[(int) Locations.Exterior, (int) Locations.Interior] == Dimensions.False &&
                    matrix[(int) Locations.Exterior, (int) Locations.Boundary] == Dimensions.False;
        }

        /// <summary> 
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is T*F**FFF*.
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="IGeometry"/>.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="IGeometry"/>.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="IGeometry"/>
        /// s related by this <see cref="IntersectionMatrix" /> are equal; the
        /// <see cref="IGeometry"/>s must have the same dimension for this function
        /// to return <c>true</c>.
        /// </returns>
        public bool IsEquals(Dimensions dimensionOfGeometryA, Dimensions dimensionOfGeometryB)
        {
            if (dimensionOfGeometryA != dimensionOfGeometryB)
                return false;            

            return Matches( matrix[(int) Locations.Interior,(int)Locations.Interior], 'T') &&
                            matrix[(int) Locations.Exterior, (int) Locations.Interior] == Dimensions.False &&
                            matrix[(int) Locations.Interior, (int) Locations.Exterior] == Dimensions.False &&
                            matrix[(int) Locations.Exterior, (int) Locations.Boundary] == Dimensions.False &&
                            matrix[(int) Locations.Boundary, (int) Locations.Exterior] == Dimensions.False;
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="IntersectionMatrix" /> is
        ///  T*T***T** (for two points or two surfaces)
        ///  1*T***T** (for two curves).
        /// </summary>
        /// <param name="dimensionOfGeometryA">The dimension of the first <see cref="IGeometry"/>.</param>
        /// <param name="dimensionOfGeometryB">The dimension of the second <see cref="IGeometry"/>.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="IGeometry"/>
        /// s related by this <see cref="IntersectionMatrix" /> overlap. For this
        /// function to return <c>true</c>, the <see cref="IGeometry"/>s must
        /// be two points, two curves or two surfaces.
        /// </returns>
        public bool IsOverlaps(Dimensions dimensionOfGeometryA, Dimensions dimensionOfGeometryB)
        {
            if ((dimensionOfGeometryA == Dimensions.Point   && dimensionOfGeometryB == Dimensions.Point) ||
                (dimensionOfGeometryA == Dimensions.Surface && dimensionOfGeometryB == Dimensions.Surface))
                return Matches(matrix[(int) Locations.Interior, (int) Locations.Interior], 'T') &&
                       Matches(matrix[(int) Locations.Interior, (int) Locations.Exterior], 'T') &&
                       Matches(matrix[(int) Locations.Exterior, (int) Locations.Interior], 'T');            

            if (dimensionOfGeometryA == Dimensions.Curve && dimensionOfGeometryB == Dimensions.Curve)
                return matrix[(int) Locations.Interior, (int) Locations.Interior] == Dimensions.Curve &&
                       Matches(matrix[(int) Locations.Interior, (int) Locations.Exterior], 'T') &&
                       Matches(matrix[(int) Locations.Exterior, (int) Locations.Interior], 'T');

            return false;
        }

        /// <summary> 
        /// Returns whether the elements of this <see cref="IntersectionMatrix" />
        /// satisfies the required dimension symbols.
        /// </summary>
        /// <param name="requiredDimensionSymbols"> 
        /// Nine dimension symbols with which to
        /// compare the elements of this <see cref="IntersectionMatrix" />. Possible
        /// values are <c>{T, F, * , 0, 1, 2}</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if this <see cref="IntersectionMatrix" />
        /// matches the required dimension symbols.
        /// </returns>
        public bool Matches(string requiredDimensionSymbols)
        {
            if (requiredDimensionSymbols.Length != 9)
                throw new ArgumentException("Should be length 9: " + requiredDimensionSymbols);            

            for (int ai = 0; ai < 3; ai++)            
                for (int bi = 0; bi < 3; bi++)                
                    if (!Matches(matrix[ai, bi], requiredDimensionSymbols[3 * ai + bi]))                    
                        return false;                                               
            return true;
        }

        /// <summary>  
        /// Transposes this IntersectionMatrix.
        /// </summary>
        /// <returns>This <see cref="IntersectionMatrix" /> as a convenience,</returns>
        public IntersectionMatrix Transpose()
        {
            Dimensions temp = matrix[1, 0];
            matrix[1, 0] = matrix[0, 1];
            matrix[0, 1] = temp;

            temp = matrix[2, 0];
            matrix[2, 0] = matrix[0, 2];
            matrix[0, 2] = temp;

            temp = matrix[2, 1];
            matrix[2, 1] = matrix[1, 2];
            matrix[1, 2] = temp;

            return this;
        }

        /// <summary>
        /// Returns a nine-character <c>String</c> representation of this <see cref="IntersectionMatrix" />.
        /// </summary>
        /// <returns>
        /// The nine dimension symbols of this <see cref="IntersectionMatrix" />
        /// in row-major order.
        /// </returns>
        public override string ToString()
        {
            StringBuilder buf = new StringBuilder("123456789");
            for (int ai = 0; ai < 3; ai++)            
                for (int bi = 0; bi < 3; bi++)                
                    buf[3 * ai + bi] =  Dimension.ToDimensionSymbol((Dimensions)matrix[ai,bi]);                            
            return buf.ToString();
        }
    }
}
