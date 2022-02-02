// Copyright (c) 2017, Oracle and/or its affiliates. All rights reserved.
//
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

using System.Reflection;
using System.Text.RegularExpressions;

namespace MySql.Utility.RegularExpressions
{
  class Program
  {
    #region Common

    /// <summary>
    /// The regular expression used to identify a captured decimal number.
    /// </summary>
    private const string CAPTURED_DECIMAL_NUMBER_REGEX = "(?<DecimalNumber>" + DECIMAL_NUMBER_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify a decimal number using "standard" western decimal point separator (.).
    /// </summary>
    private const string  DECIMAL_NUMBER_REGEX = @"-?\d+(?:\.\d+)?";

    /// <summary>
    /// The regular expression used to identify a coordinate in GML format.
    /// </summary>
    private const string GEOJSON_COORDINATE_REGEX = @"\s*\[\s*" + CAPTURED_DECIMAL_NUMBER_REGEX + @"\s*,\s*" + CAPTURED_DECIMAL_NUMBER_REGEX + @"(?:\s*,\s*" + DECIMAL_NUMBER_REGEX + @")?\s*\]\s*";

    /// <summary>
    /// The regular expression used to identify a generic XML attribute in GML.
    /// </summary>
    private const string GML_GENERIC_ATTRIBUTES = @"(?:\s*\w+\s*=\s*(?<Quote>""|')(?:[^<>&""'])*\k<Quote>)*\s*";

    /// <summary>
    /// The regular expression used to identify a coordinate in GML format.
    /// </summary>
    private const string GML_COORDINATE_REGEX = CAPTURED_DECIMAL_NUMBER_REGEX + @"\s*,\s*" + CAPTURED_DECIMAL_NUMBER_REGEX + @"(?:\s*,\s*" + DECIMAL_NUMBER_REGEX + @")?";

    /// <summary>
    /// The regular expression used to identify a position in GML format.
    /// </summary>
    private const string GML_POSITION_REGEX = CAPTURED_DECIMAL_NUMBER_REGEX + @"\s+" + CAPTURED_DECIMAL_NUMBER_REGEX + @"(?:\s+" + DECIMAL_NUMBER_REGEX + @")?";

    /// <summary>
    /// The regular expression used to identify a coordinate in KML format.
    /// </summary>
    private const string KML_COORDINATE_REGEX = CAPTURED_DECIMAL_NUMBER_REGEX + @"," + CAPTURED_DECIMAL_NUMBER_REGEX + @"(?:," + DECIMAL_NUMBER_REGEX + @")?";

    /// <summary>
    /// The regular expression used to identify a coordinate in WKT format.
    /// </summary>
    private const string WKT_COORDINATE_REGEX = @"\s*" + CAPTURED_DECIMAL_NUMBER_REGEX + @"\s+" + CAPTURED_DECIMAL_NUMBER_REGEX + @"(?:\s+" + DECIMAL_NUMBER_REGEX + @")?\s*";

    /// <summary>
    /// The regular expression used to identify an empty geometry in WKT format.
    /// </summary>
    private const string WKT_EMPTY_GEOM_REGEX = @"(?(?<=<Class>)\s+|\s*)EMPTY|\s*\(\s*\)";

    /// <summary>
    /// The regular expression used to identify a SRID in WKT format.
    /// </summary>
    private const string WKT_SRID_REGEX = @"(SRID\s*=\s*(?<SRID>\d+)\s*;\s*)?";

    #endregion Common

    #region Point

    /// <summary>
    /// The regular expression used to identify a Point coordinate in GeoJSON format.
    /// </summary>
    private const string GEOJSON_POINT_COORDINATE_REGEX = "(?<PointCoord>" + GEOJSON_COORDINATE_REGEX + @"|\s*\[\s*\]\s*)";

    /// <summary>
    /// The regular expression used to identify a Point's coordinates set in GeoJSON format.
    /// </summary>
    private const string GEOJSON_POINT_COORDINATE_SET_REGEX = @"\s*,\s*(?<Quote>""|')coordinates\k<Quote>\s*:" + GEOJSON_POINT_COORDINATE_REGEX;

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a Point class.
    /// </summary>
    private const string GEOJSON_POINT_REGEX = @"(?<BasicGeometry>\s*\{\s*(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')(?<Class>Point)\k<Quote>" + GEOJSON_POINT_COORDINATE_SET_REGEX + @"\}\s*)";

    /// <summary>
    /// The regular expression used to identify a Point coordinate in GML format.
    /// </summary>
    private const string GML_POINT_COORDINATE_REGEX = "(?<PointCoord>" + GML_POINT_COORDINATES_REGEX + "|" + GML_POS_REGEX + "|" + GML_POINT_POSLIST_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify a Point's coordinates set in KML format.
    /// </summary>
    private const string GML_POINT_COORDINATE_SET_REGEX = @"(?:" + GML_POINT_COORDINATE_REGEX + @"|\s*)";

    /// <summary>
    /// The regular expression used to identify a Point's coordinates set in GML format.
    /// </summary>
    private const string GML_POINT_COORDINATES_REGEX = @"\s*<gml:coordinates" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_COORDINATE_REGEX + @"\s*</gml:coordinates>\s*";

    /// <summary>
    /// The regular expression used to identify a position in GML format.
    /// </summary>
    private const string GML_POS_REGEX = @"\s*<gml:pos" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_POSITION_REGEX + @"\s*</gml:pos>\s*";

    /// <summary>
    /// The regular expression used to identify a Point's positions list in GML format.
    /// </summary>
    private const string GML_POINT_POSLIST_REGEX = @"\s*<gml:posList" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_POSITION_REGEX + @"\s*</gml:posList>\s*";

    /// <summary>
    /// The regular expression used to identify a Point class in GML format.
    /// </summary>
    private const string GML_POINT_REGEX = @"(?<BasicGeometry><gml:(?<Class>Point)" + GML_GENERIC_ATTRIBUTES + ">" + GML_POINT_COORDINATE_SET_REGEX + @"</gml:Point>\s*)";

    /// <summary>
    /// The regular expression used to identify a Point coordinate in KML format.
    /// </summary>
    private const string KML_POINT_COORDINATE_REGEX = "(?<PointCoord>" + KML_COORDINATE_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify a Point's coordinates set in KML format.
    /// </summary>
    private const string KML_POINT_COORDINATE_SET_REGEX = @"\s*<coordinates>\s*" + KML_POINT_COORDINATE_REGEX + @"\s*</coordinates>\s*";

    /// <summary>
    /// The regular expression used to identify a Point class in KML format.
    /// </summary>
    private const string KML_POINT_REGEX = @"(?<BasicGeometry>\s*<(?<Class>Point)>(?:" + KML_POINT_COORDINATE_SET_REGEX + @"|\s*)</Point>\s*)";

    /// <summary>
    /// The regular expression used to identify a Point coordinate in WKT format.
    /// </summary>
    private const string WKT_POINT_COORDINATE_REGEX = "(?<PointCoord>" + WKT_EMPTY_GEOM_REGEX + @"|\s*\(\s*" + WKT_COORDINATE_REGEX + @"\)\s*)";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a Point class.
    /// </summary>
    private const string WKT_POINT_REGEX = @"(?<BasicGeometry>\s*" + WKT_SRID_REGEX + @"(?<Class>POINT)" + WKT_POINT_COORDINATE_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a Point class.
    /// </summary>
    private const string WKT_POINT_REGEX_NO_SRID = @"(?<BasicGeometry>\s*(?<Class>POINT)" + WKT_POINT_COORDINATE_REGEX + ")";

    #endregion Point

    #region LineString

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a LineString class.
    /// </summary>
    private const string GEOJSON_LINESTRING_COORDINATE_SET_REGEX = @"(?<LineStringCoords>\s*\[(?:(?:" + GEOJSON_COORDINATE_REGEX + @"(?:," + GEOJSON_COORDINATE_REGEX + @")+)|\s*)\]\s*)";

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a LineString class.
    /// </summary>
    private const string GEOJSON_LINESTRING_REGEX = @"(?<BasicGeometry>\s*\{\s*(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')(?<Class>LineString)\k<Quote>\s*,\s*(?<Quote>""|')coordinates\k<Quote>\s*:" + GEOJSON_LINESTRING_COORDINATE_SET_REGEX + @"\}\s*)";

    /// <summary>
    /// The regular expression used to identify a LineString's coordinates set in GML format.
    /// </summary>
    private const string GML_LINESTRING_COORDINATE_SET_REGEX = "(?<LineStringCoords>" + GML_LINESTRING_COORDINATES_REGEX + "|" + GML_LINESTRING_POSLIST_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify a LineString's coordinates set in GML format.
    /// </summary>
    private const string GML_LINESTRING_COORDINATES_REGEX = @"\s*<gml:coordinates" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_COORDINATE_REGEX + @"(?:\s+" + GML_COORDINATE_REGEX + @")+\s*</gml:coordinates>\s*";

    /// <summary>
    /// The regular expression used to identify a LineString's positions list in GML format.
    /// </summary>
    private const string GML_LINESTRING_POSLIST_REGEX = @"\s*<gml:posList" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_POSITION_REGEX + @"(?:\s+" + GML_POSITION_REGEX + @")+\s*</gml:posList>\s*";

    /// <summary>
    /// The regular expression used to identify a LineString class in GML format.
    /// </summary>
    private const string GML_LINESTRING_REGEX = @"(?<BasicGeometry><gml:(?<Class>LineString)" + GML_GENERIC_ATTRIBUTES + ">(?:" + GML_LINESTRING_COORDINATE_SET_REGEX + @"|\s*)</gml:LineString>\s*)";

    /// <summary>
    /// The regular expression used to identify a LineString's coordinates set in KML format.
    /// </summary>
    private const string KML_LINESTRING_COORDINATE_SET_REGEX = @"(?<LineStringCoords>\s*<coordinates>\s*" + KML_COORDINATE_REGEX + @"(?:\s" + KML_COORDINATE_REGEX + @")+\s*</coordinates>\s*)";

    /// <summary>
    /// The regular expression used to identify the KML representation of a LineString class.
    /// </summary>
    private const string KML_LINESTRING_REGEX = @"(?<BasicGeometry>\s*<(?<Class>LineString)>(?:" + KML_LINESTRING_COORDINATE_SET_REGEX + @"|\s*)</LineString>\s*)";

    /// <summary>
    /// The regular expression used to identify a LineString's coordinates set in WKT format.
    /// </summary>
    private const string WKT_LINESTRING_COORDINATE_SET_REGEX = @"(?<LineStringCoords>(?:" + WKT_EMPTY_GEOM_REGEX + @"|\s*\(" + WKT_COORDINATE_REGEX + @"(?:," + WKT_COORDINATE_REGEX + @")+\)\s*))";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a LineString class.
    /// </summary>
    private const string WKT_LINESTRING_REGEX = @"(?<BasicGeometry>\s*" + WKT_SRID_REGEX + @"(?<Class>LINESTRING)" + WKT_LINESTRING_COORDINATE_SET_REGEX + @")";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a LineString class.
    /// </summary>
    private const string WKT_LINESTRING_REGEX_NO_SRID = @"(?<BasicGeometry>\s*(?<Class>LINESTRING)" + WKT_LINESTRING_COORDINATE_SET_REGEX + @")";

    #endregion LineString

    #region LinearRing

    /// <summary>
    /// The regular expression used to identify a Polygon's ring coordinates set in GML format.
    /// </summary>
    private const string GEOJSON_LINEARRING_REGEX = @"\s*(?<RingCoords>\[" + GEOJSON_COORDINATE_REGEX + "(?:," + GEOJSON_COORDINATE_REGEX + @"){3,}\])\s*";

    /// <summary>
    /// The regular expression used to identify a Polygon's ring coordinates set in GML format.
    /// </summary>
    private const string GML_LINEARRING_REGEX = @"\s*(?<RingCoords><gml:LinearRing" + GML_GENERIC_ATTRIBUTES + ">(" + GML_RING_COORDINATES_REGEX + "|" + GML_RING_POSLIST_REGEX + @")</gml:LinearRing>)\s*";

    /// <summary>
    /// The regular expression used to identify a LineString's coordinates set in GML format.
    /// </summary>
    private const string GML_RING_COORDINATES_REGEX = @"\s*<gml:coordinates" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_COORDINATE_REGEX + @"(?:\s+" + GML_COORDINATE_REGEX + @"){3,}\s*</gml:coordinates>\s*";

    /// <summary>
    /// The regular expression used to identify a LineString's positions list in GML format.
    /// </summary>
    private const string GML_RING_POSLIST_REGEX = @"\s*<gml:posList" + GML_GENERIC_ATTRIBUTES + @">\s*" + GML_POSITION_REGEX + @"(?:\s+" + GML_POSITION_REGEX + @"){3,}\s*</gml:posList>\s*";

    /// <summary>
    /// The regular expression used to identify a Polygon's ring coordinates set in KML format.
    /// </summary>
    private const string KML_LINEARRING_REGEX = @"\s*(?<RingCoords><LinearRing>\s*<coordinates>\s*" + KML_COORDINATE_REGEX + @"(?:\s" + KML_COORDINATE_REGEX + @"){3,}\s*</coordinates>\s*</LinearRing>)\s*";

    /// <summary>
    /// The regular expression used to identify a Polygon's ring coordinates set in WKT format.
    /// </summary>
    private const string WKT_LINEARRING_REGEX = @"\s*(?<RingCoords>\(" + WKT_COORDINATE_REGEX + @"(?:," + WKT_COORDINATE_REGEX + @"){3,}\))\s*";

    #endregion LinearRing

    #region Polygon

    /// <summary>
    /// The regular expression used to identify a Polygon's coordinates set in GeoJSON format.
    /// </summary>
    private const string GEOJSON_POLYGON_COORDINATE_SET_REGEX = @"(?<PolygonRings>\s*\[(?:(?:" + GEOJSON_LINEARRING_REGEX + @"(?:," + GEOJSON_LINEARRING_REGEX + @")*)|\s*)\]\s*)";

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a Polygon class.
    /// </summary>
    private const string GEOJSON_POLYGON_REGEX = @"(?<BasicGeometry>\s*\{\s*(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')(?<Class>Polygon)\k<Quote>\s*,\s*(?<Quote>""|')coordinates\k<Quote>\s*:" + GEOJSON_POLYGON_COORDINATE_SET_REGEX + @"\}\s*)";

    /// <summary>
    /// The regular expression used to identify a Polygon's coordinates set in GML format.
    /// </summary>
    private const string GML_POLYGON_COORDINATE_SET_REGEX = @"(?<PolygonRings>\s*<gml:outerBoundaryIs" + GML_GENERIC_ATTRIBUTES + ">" + GML_LINEARRING_REGEX + @"</gml:outerBoundaryIs>\s*(<gml:innerBoundaryIs" + GML_GENERIC_ATTRIBUTES + ">" + GML_LINEARRING_REGEX + @"</gml:innerBoundaryIs>\s*)*)";

    /// <summary>
    /// The regular expression used to identify the GML representation of a Polygon class.
    /// </summary>
    private const string GML_POLYGON_REGEX = @"(?<BasicGeometry>\s*<gml:(?<Class>Polygon)" + GML_GENERIC_ATTRIBUTES + ">(?:" + GML_POLYGON_COORDINATE_SET_REGEX + @"|\s*)</gml:Polygon>\s*)";

    /// <summary>
    /// The regular expression used to identify a Polygon's coordinates set in KML format.
    /// </summary>
    private const string KML_POLYGON_COORDINATE_SET_REGEX = @"(?<PolygonRings>\s*<outerBoundaryIs>" + KML_LINEARRING_REGEX + @"</outerBoundaryIs>\s*(<innerBoundaryIs>" + KML_LINEARRING_REGEX + @"</innerBoundaryIs>\s*)*)";

    /// <summary>
    /// The regular expression used to identify the KML representation of a Polygon class.
    /// </summary>
    private const string KML_POLYGON_REGEX = @"(?<BasicGeometry>\s*<(?<Class>Polygon)>(?:" + KML_POLYGON_COORDINATE_SET_REGEX + @"|\s*)</Polygon>\s*)";

    /// <summary>
    /// The regular expression used to identify a Polygon's ring coordinates set in WKT format.
    /// </summary>
    private const string WKT_POLYGON_COORDINATE_SET_REGEX = @"(?<PolygonRings>" + WKT_EMPTY_GEOM_REGEX + @"|\s*\(" + WKT_LINEARRING_REGEX + "(?:," + WKT_LINEARRING_REGEX + ")*" + @"\)\s*)";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a Polygon class.
    /// </summary>
    private const string WKT_POLYGON_REGEX = @"(?<BasicGeometry>\s*" + WKT_SRID_REGEX + @"(?<Class>POLYGON)" + WKT_POLYGON_COORDINATE_SET_REGEX + @")";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a Polygon class.
    /// </summary>
    private const string WKT_POLYGON_REGEX_NO_SRID = @"(?<BasicGeometry>\s*(?<Class>POLYGON)" + WKT_POLYGON_COORDINATE_SET_REGEX + @")";

    #endregion Polygon

    #region MultiPoint

    /// <summary>
    /// The regular expression used to identify a MultiPoint's coordinates set in GeoJSON format.
    /// </summary>
    private const string GEOJSON_MULTIPOINT_COORDINATE_SET_REGEX = @"\s*\[(?:(?:" + GEOJSON_POINT_COORDINATE_REGEX + @"(?:," + GEOJSON_POINT_COORDINATE_REGEX + @")*)|\s*)\]\s*";

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a MultiPoint class.
    /// </summary>
    private const string GEOJSON_MULTIPOINT_REGEX = @"\s*\{\s*(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')MultiPoint\k<Quote>\s*,\s*(?<Quote>""|')coordinates\k<Quote>\s*:" + GEOJSON_MULTIPOINT_COORDINATE_SET_REGEX + @"\}\s*";

    /// <summary>
    /// The regular expression used to identify a MultiPoint's elements set in GML format.
    /// </summary>
    private const string GML_MULTIPOINT_ELEMENTS_SET_REGEX = @"(?:\s*<gml:pointMember" + GML_GENERIC_ATTRIBUTES + ">" + GML_POINT_REGEX + @"</gml:pointMember>\s*)*";

    /// <summary>
    /// The regular expression used to identify the GML representation of a MultiPoint class.
    /// </summary>
    private const string GML_MULTIPOINT_REGEX = @"<gml:(?<Class>MultiPoint)" + GML_GENERIC_ATTRIBUTES + ">" + GML_MULTIPOINT_ELEMENTS_SET_REGEX + @"</gml:MultiPoint>\s*";

    /// <summary>
    /// The regular expression used to identify a single coordinate within a MultiPoint in WKT format.
    /// </summary>
    private const string WKT_MULTIPOINT_SINGLE_COORDINATE_REGEX = @"(?<PointCoord>\s*(?:EMPTY|" + WKT_COORDINATE_REGEX + @")\s*)";

    /// <summary>
    /// The regular expression used to identify a single coordinate within a MultiPoint in WKT format.
    /// </summary>
    private const string WKT_MULTIPOINT_SINGLE_COORDINATE_OR_EMPTY_REGEX = @"(?:\s*|" + WKT_MULTIPOINT_SINGLE_COORDINATE_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify a MultiPoint's ring coordinates set in WKT format.
    /// </summary>
    private const string WKT_MULTIPOINT_COORDINATE_SET_REGEX = @"\s*\(" + WKT_MULTIPOINT_SINGLE_COORDINATE_OR_EMPTY_REGEX + @"(?:," + WKT_MULTIPOINT_SINGLE_COORDINATE_OR_EMPTY_REGEX + @")*\)";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a MultiPoint class.
    /// </summary>
    private const string WKT_MULTIPOINT_REGEX = @"\s*" + WKT_SRID_REGEX + @"(?<Class>MULTIPOINT)(?:" + WKT_EMPTY_GEOM_REGEX + @"|" + WKT_MULTIPOINT_COORDINATE_SET_REGEX + ")";

    #endregion MultiPoint

    #region MultiLineString

    /// <summary>
    /// The regular expression used to identify a MultiLineString's coordinates set in GeoJSON format.
    /// </summary>
    private const string GEOJSON_MULTILINESTRING_COORDINATE_SET_REGEX = @"\s*\[(?:(?:" + GEOJSON_LINESTRING_COORDINATE_SET_REGEX + @"(?:," + GEOJSON_LINESTRING_COORDINATE_SET_REGEX + @")*)|\s*)\]\s*";

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a MultiLineString class.
    /// </summary>
    private const string GEOJSON_MULTILINESTRING_REGEX = @"\s*\{\s(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')MultiLineString\k<Quote>\s*,\s*(?<Quote>""|')coordinates\k<Quote>\s*:" + GEOJSON_MULTILINESTRING_COORDINATE_SET_REGEX + @"\}\s*";

    /// <summary>
    /// The regular expression used to identify a MultiLineString's elements set in GML format.
    /// </summary>
    private const string GML_MULTILINESTRING_ELEMENTS_SET_REGEX = @"(?:\s*<gml:lineStringMember" + GML_GENERIC_ATTRIBUTES + ">" + GML_LINESTRING_REGEX + @"</gml:lineStringMember>\s*)*";

    /// <summary>
    /// The regular expression used to identify the GML representation of a MultiLineString class.
    /// </summary>
    private const string GML_MULTILINESTRING_REGEX = @"<gml:(?<Class>MultiLineString)" + GML_GENERIC_ATTRIBUTES + ">" + GML_MULTILINESTRING_ELEMENTS_SET_REGEX + @"</gml:MultiLineString>\s*";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a MultiLineString class.
    /// </summary>
    private const string WKT_MULTILINESTRING_REGEX = @"\s*" + WKT_SRID_REGEX + @"(?<Class>MULTILINESTRING)(?:" + WKT_EMPTY_GEOM_REGEX + @"|\s*\(" + WKT_LINESTRING_COORDINATE_SET_REGEX + @"(?:," + WKT_LINESTRING_COORDINATE_SET_REGEX + ")*" + @"\))";

    #endregion MultiLineString

    #region MultiPolygon

    /// <summary>
    /// The regular expression used to identify a MultiPolygon's coordinates set in GeoJSON format.
    /// </summary>
    private const string GEOJSON_MULTIPOLYGON_COORDINATE_SET_REGEX = @"\s*\[(?:(?:" + GEOJSON_POLYGON_COORDINATE_SET_REGEX + @"(?:," + GEOJSON_POLYGON_COORDINATE_SET_REGEX + @")*)|\s*)\]\s*";

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a MultiPolygon class.
    /// </summary>
    private const string GEOJSON_MULTIPOLYGON_REGEX = @"\s*\{\s(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')MultiPolygon\k<Quote>\s*,\s*(?<Quote>""|')coordinates\k<Quote>\s*:" + GEOJSON_MULTIPOLYGON_COORDINATE_SET_REGEX + @"\}\s*";

    /// <summary>
    /// The regular expression used to identify a MultiPolygon's elements set in GML format.
    /// </summary>
    private const string GML_MULTIPOLYGON_ELEMENTS_SET_REGEX = @"(?:\s*<gml:polygonMember" + GML_GENERIC_ATTRIBUTES + ">" + GML_POLYGON_REGEX + @"</gml:polygonMember>\s*)*";

    /// <summary>
    /// The regular expression used to identify the GML representation of a MultiPolygon class.
    /// </summary>
    private const string GML_MULTIPOLYGON_REGEX = @"<gml:(?<Class>MultiPolygon)" + GML_GENERIC_ATTRIBUTES + ">" + GML_MULTIPOLYGON_ELEMENTS_SET_REGEX + @"</gml:MultiPolygon>\s*";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a MultiPolygon class.
    /// </summary>
    private const string WKT_MULTIPOLYGON_REGEX = @"\s*" + WKT_SRID_REGEX + @"(?<Class>MULTIPOLYGON)(?:" + WKT_EMPTY_GEOM_REGEX + @"|\s*\(" + WKT_POLYGON_COORDINATE_SET_REGEX + @"(?:," + WKT_POLYGON_COORDINATE_SET_REGEX + ")*" + @"\))";

    #endregion MultiPolygon

    #region GeometryCollection

    /// <summary>
    /// The regular expression used to identify a GeometryCollection's elements set in GeoJSON format.
    /// </summary>
    private const string GEOJSON_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX = @"(?:" + GEOJSON_POINT_REGEX + "|" + GEOJSON_LINESTRING_REGEX + "|" + GEOJSON_POLYGON_REGEX + @")";

    /// <summary>
    /// The regular expression used to identify the GeoJSON representation of a GeometryCollection class.
    /// </summary>
    private const string GEOJSON_GEOMETRYCOLLECTION_REGEX = @"\s*\{\s*(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')GeometryCollection\k<Quote>\s*,\s*(?<Quote>""|')geometries\k<Quote>\s*:\s*\[(?:\s*|" + GEOJSON_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX + @"(?:," + GEOJSON_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX + @")*)\]\s*\}\s*";

    /// <summary>
    /// The regular expression used to identify a GeometryCollection's elements set in GML format.
    /// </summary>
    private const string GML_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX = @"(?:\s*<gml:geometryMember" + GML_GENERIC_ATTRIBUTES + ">(?:" + GML_POINT_REGEX + "|" + GML_LINESTRING_REGEX + "|" + GML_POLYGON_REGEX + @")</gml:geometryMember>\s*)*";

    /// <summary>
    /// The regular expression used to identify the GML representation of a GeometryCollection class.
    /// </summary>
    private const string GML_GEOMETRYCOLLECTION_REGEX = @"<gml:(?<Class>MultiGeometry)" + GML_GENERIC_ATTRIBUTES + ">" + GML_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX + @"</gml:MultiGeometry>\s*";

    /// <summary>
    /// The regular expression used to identify a GeometryCollection's elements set in KML format.
    /// </summary>
    private const string KML_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX = "(?:" + KML_POINT_REGEX + "|" + KML_LINESTRING_REGEX + "|" + KML_POLYGON_REGEX + ")";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a GeometryCollection class.
    /// </summary>
    private const string KML_GEOMETRYCOLLECTION_REGEX = @"\s*<(?<Class>MultiGeometry)>" + KML_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX + @"*</MultiGeometry>\s*";

    /// <summary>
    /// The regular expression used to identify the geometries allowed inside a GeometryCollection in WKT format.
    /// </summary>
    private const string WKT_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX = "(?:" + WKT_POINT_REGEX_NO_SRID + "|" + WKT_LINESTRING_REGEX_NO_SRID + "|" + WKT_POLYGON_REGEX_NO_SRID + ")";

    /// <summary>
    /// The regular expression used to identify the WKT representation of a GeometryCollection class.
    /// </summary>
    private const string WKT_GEOMETRYCOLLECTION_REGEX = @"\s*" + WKT_SRID_REGEX + @"(?<Class>GEOMETRYCOLLECTION)(?:" + WKT_EMPTY_GEOM_REGEX + @"|\s*\(" + WKT_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX + @"(?:," + WKT_GEOMETRYCOLLECTION_ELEMENTS_SET_REGEX + @")*\))";

    #endregion GeometryCollection

    #region AllowedGeoms

    /// <summary>
    /// The regular expression used to identify the WKT allowed geometries.
    /// </summary>
    private const string GEOJSON_ALLOWED_GEOMS_REGEX = @"\s*\{\s(?<Quote>""|')type\k<Quote>\s*:\s*(?<Quote>""|')(?<Class>Point|LineString|Polygon|MultiPoint|MultiLineString|MultiPolygon|GeometryCollection)\k<Quote>.*\}";

    /// <summary>
    /// The regular expression used to identify the WKT allowed geometries.
    /// </summary>
    private const string GML_ALLOWED_GEOMS_REGEX = @"^\s*<gml:(?<Class>Point|LineString|Polygon|MultiPoint|MultiLineString|MultiPolygon|MultiGeometry).*>";

    /// <summary>
    /// The regular expression used to identify the WKT allowed geometries.
    /// </summary>
    private const string KML_ALLOWED_GEOMS_REGEX = @"^\s*<(?<Class>Point|LineString|Polygon|MultiGeometry)>";

    /// <summary>
    /// The regular expression used to identify the WKT allowed geometries.
    /// </summary>
    private const string WKT_ALLOWED_GEOMS_REGEX = @"^\s*" + WKT_SRID_REGEX + @"(?<Class>POINT|LINESTRING|POLYGON|MULTIPOINT|MULTILINESTRING|MULTIPOLYGON|GEOMETRYCOLLECTION)";

    #endregion AllowedGeoms

    static void Main(string[] args)
    {
      var wktPointRegex = new RegexCompilationInfo(WKT_POINT_REGEX, RegexOptions.IgnoreCase, "WktPointRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlPointRegex = new RegexCompilationInfo(KML_POINT_REGEX, RegexOptions.IgnoreCase, "KmlPointRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlPointRegex = new RegexCompilationInfo(GML_POINT_REGEX, RegexOptions.IgnoreCase, "GmlPointRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonPointRegex = new RegexCompilationInfo(GEOJSON_POINT_REGEX, RegexOptions.IgnoreCase, "GeoJsonPointRegex", "MySql.Utility.Classes.Spatial", true);
      var wktLineStringRegex = new RegexCompilationInfo(WKT_LINESTRING_REGEX, RegexOptions.IgnoreCase, "WktLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlLineStringRegex = new RegexCompilationInfo(KML_LINESTRING_REGEX, RegexOptions.IgnoreCase, "KmlLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlLineStringRegex = new RegexCompilationInfo(GML_LINESTRING_REGEX, RegexOptions.IgnoreCase, "GmlLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonLineStringRegex = new RegexCompilationInfo(GEOJSON_LINESTRING_REGEX, RegexOptions.IgnoreCase, "GeoJsonLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var wktPolygonRegex = new RegexCompilationInfo(WKT_POLYGON_REGEX, RegexOptions.IgnoreCase, "WktPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlPolygonRegex = new RegexCompilationInfo(KML_POLYGON_REGEX, RegexOptions.IgnoreCase, "KmlPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlPolygonRegex = new RegexCompilationInfo(GML_POLYGON_REGEX, RegexOptions.IgnoreCase, "GmlPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonPolygonRegex = new RegexCompilationInfo(GEOJSON_POLYGON_REGEX, RegexOptions.IgnoreCase, "GeoJsonPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var wktMultiPointRegex = new RegexCompilationInfo(WKT_MULTIPOINT_REGEX, RegexOptions.IgnoreCase, "WktMultiPointRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlMultiPointRegex = new RegexCompilationInfo(GML_MULTIPOINT_REGEX, RegexOptions.IgnoreCase, "GmlMultiPointRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonMultiPointRegex = new RegexCompilationInfo(GEOJSON_MULTIPOINT_REGEX, RegexOptions.IgnoreCase, "GeoJsonMultiPointRegex", "MySql.Utility.Classes.Spatial", true);
      var wktMultiLineStringRegex = new RegexCompilationInfo(WKT_MULTILINESTRING_REGEX, RegexOptions.IgnoreCase, "WktMultiLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlMultiLineStringRegex = new RegexCompilationInfo(GML_MULTILINESTRING_REGEX, RegexOptions.IgnoreCase, "GmlMultiLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonMultiLineStringRegex = new RegexCompilationInfo(GEOJSON_MULTILINESTRING_REGEX, RegexOptions.IgnoreCase, "GeoJsonMultiLineStringRegex", "MySql.Utility.Classes.Spatial", true);
      var wktMultiPolygonRegex = new RegexCompilationInfo(WKT_MULTIPOLYGON_REGEX, RegexOptions.IgnoreCase, "WktMultiPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlMultiPolygonRegex = new RegexCompilationInfo(GML_MULTIPOLYGON_REGEX, RegexOptions.IgnoreCase, "GmlMultiPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonMultiPolygonRegex = new RegexCompilationInfo(GEOJSON_MULTIPOLYGON_REGEX, RegexOptions.IgnoreCase, "GeoJsonMultiPolygonRegex", "MySql.Utility.Classes.Spatial", true);
      var wktGeometryCollectionRegex = new RegexCompilationInfo(WKT_GEOMETRYCOLLECTION_REGEX, RegexOptions.IgnoreCase, "WktGeometryCollectionRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlGeometryCollectionRegex = new RegexCompilationInfo(KML_GEOMETRYCOLLECTION_REGEX, RegexOptions.IgnoreCase, "KmlGeometryCollectionRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlGeometryCollectionRegex = new RegexCompilationInfo(GML_GEOMETRYCOLLECTION_REGEX, RegexOptions.IgnoreCase, "GmlGeometryCollectionRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonGeometryCollectionRegex = new RegexCompilationInfo(GEOJSON_GEOMETRYCOLLECTION_REGEX, RegexOptions.IgnoreCase, "GeoJsonGeometryCollectionRegex", "MySql.Utility.Classes.Spatial", true);
      var wktAllowedGeomClassesRegex = new RegexCompilationInfo(WKT_ALLOWED_GEOMS_REGEX, RegexOptions.IgnoreCase, "WktAllowedGeomClassesRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlAllowedGeomClassesRegex = new RegexCompilationInfo(KML_ALLOWED_GEOMS_REGEX, RegexOptions.IgnoreCase, "KmlAllowedGeomClassesRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlAllowedGeomClassesRegex = new RegexCompilationInfo(GML_ALLOWED_GEOMS_REGEX, RegexOptions.IgnoreCase, "GmlAllowedGeomClassesRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonAllowedGeomClassesRegex = new RegexCompilationInfo(GEOJSON_ALLOWED_GEOMS_REGEX, RegexOptions.IgnoreCase, "GeoJsonAllowedGeomClassesRegex", "MySql.Utility.Classes.Spatial", true);
      var wktPointCoordRegex = new RegexCompilationInfo(WKT_POINT_COORDINATE_REGEX, RegexOptions.IgnoreCase, "WktPointCoordRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlPointCoordRegex = new RegexCompilationInfo(KML_POINT_COORDINATE_REGEX, RegexOptions.IgnoreCase, "KmlPointCoordRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlPointCoordRegex = new RegexCompilationInfo(GML_POINT_COORDINATE_REGEX, RegexOptions.IgnoreCase, "GmlPointCoordRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonPointCoordRegex = new RegexCompilationInfo(GEOJSON_POINT_COORDINATE_REGEX, RegexOptions.IgnoreCase, "GeoJsonPointCoordRegex", "MySql.Utility.Classes.Spatial", true);
      var wktLinearRingCoordsRegex = new RegexCompilationInfo(WKT_LINEARRING_REGEX, RegexOptions.IgnoreCase, "WktLinearRingCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlLinearRingCoordsRegex = new RegexCompilationInfo(KML_LINEARRING_REGEX, RegexOptions.IgnoreCase, "KmlLinearRingCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlLinearRingCoordsRegex = new RegexCompilationInfo(GML_LINEARRING_REGEX, RegexOptions.IgnoreCase, "GmlLinearRingCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonLinearRingCoordsRegex = new RegexCompilationInfo(GEOJSON_LINEARRING_REGEX, RegexOptions.IgnoreCase, "GeoJsonLinearRingCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var wktLineStringCoordsRegex = new RegexCompilationInfo(WKT_LINESTRING_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "WktLineStringCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlLineStringCoordsRegex = new RegexCompilationInfo(KML_LINESTRING_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "KmlLineStringCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlLineStringCoordsRegex = new RegexCompilationInfo(GML_LINESTRING_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "GmlLineStringCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonLineStringCoordsRegex = new RegexCompilationInfo(GEOJSON_LINESTRING_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "GeoJsonLineStringCoordsRegex", "MySql.Utility.Classes.Spatial", true);
      var wktPolygonRingsRegex = new RegexCompilationInfo(WKT_POLYGON_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "WktPolygonRingsRegex", "MySql.Utility.Classes.Spatial", true);
      var kmlPolygonRingsRegex = new RegexCompilationInfo(KML_POLYGON_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "KmlPolygonRingsRegex", "MySql.Utility.Classes.Spatial", true);
      var gmlPolygonRingsRegex = new RegexCompilationInfo(GML_POLYGON_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "GmlPolygonRingsRegex", "MySql.Utility.Classes.Spatial", true);
      var geoJsonPolygonRingsRegex = new RegexCompilationInfo(GEOJSON_POLYGON_COORDINATE_SET_REGEX, RegexOptions.IgnoreCase, "GeoJsonPolygonRingsRegex", "MySql.Utility.Classes.Spatial", true);
      var wktMultiPointCoordRegex = new RegexCompilationInfo(WKT_MULTIPOINT_SINGLE_COORDINATE_REGEX, RegexOptions.IgnoreCase, "WktMultiPointCoordRegex", "MySql.Utility.Classes.Spatial", true);
      RegexCompilationInfo[] regexes =
      {
        wktPointRegex,
        kmlPointRegex,
        gmlPointRegex,
        geoJsonPointRegex,
        wktPointCoordRegex,
        kmlPointCoordRegex,
        gmlPointCoordRegex,
        geoJsonPointCoordRegex,
        wktLineStringRegex,
        kmlLineStringRegex,
        gmlLineStringRegex,
        geoJsonLineStringRegex,
        wktLineStringCoordsRegex,
        kmlLineStringCoordsRegex,
        gmlLineStringCoordsRegex,
        geoJsonLineStringCoordsRegex,
        wktLinearRingCoordsRegex,
        kmlLinearRingCoordsRegex,
        gmlLinearRingCoordsRegex,
        geoJsonLinearRingCoordsRegex,
        wktPolygonRegex,
        kmlPolygonRegex,
        gmlPolygonRegex,
        geoJsonPolygonRegex,
        wktPolygonRingsRegex,
        kmlPolygonRingsRegex,
        gmlPolygonRingsRegex,
        geoJsonPolygonRingsRegex,
        wktMultiPointRegex,
        gmlMultiPointRegex,
        geoJsonMultiPointRegex,
        wktMultiLineStringRegex,
        gmlMultiLineStringRegex,
        geoJsonMultiLineStringRegex,
        wktMultiPolygonRegex,
        gmlMultiPolygonRegex,
        geoJsonMultiPolygonRegex,
        wktGeometryCollectionRegex,
        kmlGeometryCollectionRegex,
        gmlGeometryCollectionRegex,
        geoJsonGeometryCollectionRegex,
        wktAllowedGeomClassesRegex,
        kmlAllowedGeomClassesRegex,
        gmlAllowedGeomClassesRegex,
        geoJsonAllowedGeomClassesRegex,
        wktMultiPointCoordRegex
      };
      var assemName = new AssemblyName("MySql.Utility.RegularExpressions, Version=1.0.0, Culture=neutral, PublicKeyToken=null");
      Regex.CompileToAssembly(regexes, assemName);
    }
  }
}
