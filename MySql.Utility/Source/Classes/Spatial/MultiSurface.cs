// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System.Collections.Generic;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// A <see cref="GeometryCollection"/> composed only of <see cref="Surface"/> elements.
  /// </summary>
  public abstract class MultiSurface : GeometryCollection
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSurface"/> class.
    /// </summary>
    /// <param name="surfaces">An array of <see cref="Surface"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiSurface"/>.</param>
    protected MultiSurface(Surface[] surfaces, int srid)
      : this(srid)
    {
      if (surfaces != null)
      {
        Geometries = new List<Geometry>(surfaces);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSurface"/> class.
    /// </summary>
    /// <param name="surfaces">A list of <see cref="Surface"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiSurface"/>.</param>
    protected MultiSurface(List<Surface> surfaces, int srid)
      : this (srid)
    {
      if (surfaces != null)
      {
        Geometries = new List<Geometry>(surfaces);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSurface"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="MultiSurface"/>.</param>
    protected MultiSurface(int srid)
      : base(srid)
    {
    }
  }
}
