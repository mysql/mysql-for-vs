// Copyright (c) 2013, 2019, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.MySqlWorkbench;
using MySql.Utility.Classes.MySqlX;
using MySql.Utility.Classes.Tokenizers;
using MySql.Utility.Classes.VisualStyles;
using MySql.Utility.Enums;
using MySql.Utility.Structs;
using ConnectionsMigrationDelayType = MySql.Utility.Classes.MySqlWorkbench.MySqlWorkbench.ConnectionsMigrationDelayType;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using MySql.Utility.Classes.Attributes;
using MySqlX.Shell;


namespace MySql.Utility.Classes
{
  /// <summary>
  /// Extension methods class to be used wherever the MySql.Utility is used.
  /// </summary>
  public static class ExtensionMethods
  {
    #region Constants

    /// <summary>
    /// Zero seconds formatted like execution time.
    /// </summary>
    public const string ZERO_EXECUTION_TIME = "0.00 sec";

    /// <summary>
    /// The regex pattern that can match an informational item from the Shell Client.
    /// </summary>
    internal const string CLASS_AND_VALUE_REGEX_PATTERN = "<(?<ClassID>.+):(?<Value>.+)>";

    #endregion Constants

    #region Fields

    /// <summary>
    /// A <see cref="Regex"/> to match an informational item from the Shell Client in a format containing a class ID and a value.
    /// </summary>
    private static Regex _classIdAndValueRegex;

    /// <summary>
    /// The default icon for an <see cref="ErrorProvider"/>.
    /// </summary>
    private static Icon _errorProviderDefaultIcon;

    /// <summary>
    /// The <see cref="Control.Validating"/> event method.
    /// </summary>
    private static readonly MethodInfo _onValidating = typeof(Control).GetMethod("OnValidating", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// The <see cref="Control.Validated"/> event method.
    /// </summary>
    private static readonly MethodInfo _onValidated = typeof(Control).GetMethod("OnValidated", BindingFlags.Instance | BindingFlags.NonPublic);

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets a <see cref="Regex"/> to match an informational item from the Shell Client in a format containing a class ID and a value.
    /// </summary>
    internal static Regex ClassIdAndValueRegex => _classIdAndValueRegex ?? (_classIdAndValueRegex = new Regex(CLASS_AND_VALUE_REGEX_PATTERN));

    /// <summary>
    /// Gets The default icon for an <see cref="ErrorProvider"/>.
    /// </summary>
    internal static Icon ErrorProviderDefaultIcon => _errorProviderDefaultIcon ?? (_errorProviderDefaultIcon = new Icon(typeof(ErrorProvider), "Error.ico"));

    #endregion Properties

    /// <summary>
    /// Moves the given <see cref="Control"/> simulating an animation over a period of time.
    /// </summary>
    /// <param name="control">A <see cref="Control"/> to move.</param>
    /// <param name="newLocation">The new location for the control.</param>
    /// <param name="steps">The number of steps to get to the new location.</param>
    /// <param name="animationTime">The total time (in milliseconds) the animation will take.</param>
    public static void Animate(this Control control, Point newLocation, int steps = 20, int animationTime = 1000)
    {
      if (control == null)
      {
        throw new ArgumentNullException(nameof(control));
      }

      if (newLocation == Point.Empty
          || newLocation == control.Location
          || steps == 0)
      {
        return;
      }

      steps = Math.Abs(steps);
      animationTime = Math.Abs(animationTime);
      var deltaX = (newLocation.X - control.Location.X) / steps;
      var deltaY = (newLocation.Y - control.Location.Y) / steps;
      var deltaTime = animationTime / steps;
      for (var step = 1; step < steps; step++)
      {
        control.Location = new Point(control.Location.X + deltaX, control.Location.Y + deltaY);
        control.Parent.Refresh();
        if (deltaTime > 0)
        {
          Thread.Sleep(deltaTime);
        }
      }

      control.Location = newLocation;
      control.Parent.Refresh();
    }

    /// <summary>
    /// Returns a list of statements from a JavaScript script.
    /// </summary>
    /// <param name="script">A JavaScript script.</param>
    /// <returns>A list of statements from a JavaScript script.</returns>
    public static List<string> BreakIntoJavaScriptStatements(this string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      var tokenizer = new MyJsTokenizer(script.Trim());
      return tokenizer.BreakIntoStatements();
    }

    /// <summary>
    /// Returns a list of statements from a Python script.
    /// </summary>
    /// <param name="script">A Python script.</param>
    /// <returns>A list of statements from a Python script.</returns>
    public static List<string> BreakIntoPythonStatements(this string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      var tokenizer = new MyPythonTokenizer(script.Trim());
      return tokenizer.BreakIntoStatements();
    }

    /// <summary>
    /// Returns a list of statements from a SQL script.
    /// </summary>
    /// <param name="script">A SQL script.</param>
    /// <returns>A list of statements from a SQL script.</returns>
    public static List<string> BreakIntoSqlStatements(this string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return new List<string>();
      }

      var tokenizer = new MySqlTokenizer(script.Trim());
      return tokenizer.BreakIntoStatements();
    }

    /// <summary>
    /// Returns the given string with a character in the given position converted to upper or lower case.
    /// </summary>
    /// <param name="text">A text.</param>
    /// <param name="position">The index within the text for the character to change.</param>
    /// <param name="upperCase">Flag indicating if a character is converted to upper case, otherwise to lower case.</param>
    /// <returns>The given string with a character in the given position converted to upper or lower case.</returns>
    public static string ChangeCaseAt(this string text, int position, bool upperCase)
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      if (position < 0 || position >= text.Length)
      {
        throw new IndexOutOfRangeException($"Position {position} is out of range.");
      }

      char[] a = text.ToCharArray();
      a[position] = upperCase
        ? char.ToUpper(a[position])
        : char.ToLower(a[position]);

      return new string(a);
    }

    /// <summary>
    /// Changes the casing of a given text.
    /// </summary>
    /// <param name="text">A text.</param>
    /// <param name="textCasing">A <see cref="TextCasingType"/> value.</param>
    /// <param name="onlyLettersAndDigits">Flag indicating if the resulting text only contains letters and digits.</param>
    /// <returns>A text transformed to the given casing.</returns>
    public static string ChangeCasing(this string text, TextCasingType textCasing, bool onlyLettersAndDigits = false)
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      if (textCasing != TextCasingType.LowerCase
          && textCasing != TextCasingType.UpperCase
          && textCasing != TextCasingType.TitleCase)
      {
        onlyLettersAndDigits = true;
      }

      if (onlyLettersAndDigits)
      {
        var resultBuilder = new StringBuilder();
        foreach (char c in text)
        {
          // Replace anything, but letters and digits, with space
          if (!char.IsLetterOrDigit(c))
          {
            resultBuilder.Append(" ");
          }
          else
          {
            resultBuilder.Append(c);
          }
        }

        text = resultBuilder.ToString();
      }

      switch (textCasing)
      {
        case TextCasingType.LowerCase:
          return text.ToLower();

        case TextCasingType.UpperCase:
          return text.ToUpper();

        case TextCasingType.TitleCase:
          return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());

        case TextCasingType.PascalCase:
          return ChangeCasing(text, TextCasingType.TitleCase).Replace(" ", string.Empty);

        case TextCasingType.CamelCase:
          return ChangeCasing(text, TextCasingType.PascalCase).ToLowerAt(0);

        case TextCasingType.KebabCase:
          return text.ToLower().Replace(" ", "-");

        case TextCasingType.SnakeCase:
          return text.ToLower().Replace(" ", "_");
      }

      return text;
    }

    /// <summary>
    /// Creates a new bitmap based on a given bitmap with its colors changed using the given <see cref="ColorMatrix"/>.
    /// </summary>
    /// <param name="original">The bitmap to change.</param>
    /// <param name="colorMatrix">The <see cref="ColorMatrix"/> used to alter the colors in the original bitmap.</param>
    /// <returns>A new bitmap based on a given bitmap with its colors changed using the given <see cref="ColorMatrix"/>.</returns>
    public static Bitmap ChangeColors(this Bitmap original, ColorMatrix colorMatrix)
    {
      if (original == null)
      {
        return null;
      }

      if (colorMatrix == null)
      {
        return original;
      }

      // Create a blank bitmap the same size as original
      Bitmap newBitmap = new Bitmap(original.Width, original.Height);

      // Get a graphics object from the new image
      using (Graphics g = Graphics.FromImage(newBitmap))
      {
        // Create some image attributes
        ImageAttributes attributes = new ImageAttributes();

        // Set the color matrix attribute
        attributes.SetColorMatrix(colorMatrix);

        // Draw the original image on the new image using the grayscale color matrix
        g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
           0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
      }

      return newBitmap;
    }

    /// <summary>
    /// Attempts to convert a collection in string representation into a list of dictionaries of results and information about them.
    /// </summary>
    /// <param name="collectionString">A collection in string representation.</param>
    /// <param name="stripClassId">Flag indicating whether class identifiers are stripped from the collection.</param>
    /// <returns>A list of dictionaries of results and information about them.</returns>
    public static List<Dictionary<string, object>> CollectionToDictionaryList(this string collectionString, bool stripClassId)
    {
      collectionString = collectionString.Replace("\n","");

      if (!collectionString.IsCollection())
      {
        return null;
      }

      var collection = collectionString.Trim('[', ']', ' ');
      List<string> collectionList;
      if (collection.StartsWith("{", StringComparison.Ordinal) &&
          collection.EndsWith("}", StringComparison.Ordinal))
      {
        collection = collection.Replace("},{", "}{");
        collection = collection.Replace("}","");
        collectionList = collection.Split('{').ToList();
        collectionList = collectionList.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
      }
      else
      {
        collectionList = collection.Split(',').ToList();
      }

      var itemsCount = collectionList.Count();
      var dictionariesList = new List<Dictionary<string, object>>(itemsCount);
      foreach (var item in collectionList)
      {
        var classIdAndValue = (stripClassId
          ? item.GetClassIdAndValue()
          : null) ?? new Tuple<string, string>("string", item);
        var itemDictionary = new Dictionary<string, object>
        {
          { classIdAndValue.Item1, classIdAndValue.Item2 }
        };
        dictionariesList.Add(itemDictionary);
      }

      return dictionariesList;
    }

    /// <summary>
    /// Compares two <see cref="DbConnectionStringBuilder"/> instances to see if they are similar by checking their core host parameters (server, port, user id and database).
    /// </summary>
    /// <param name="sourceConnectionStringBuilder">The source connection string builder.</param>
    /// <param name="targetConnectionStringBuilder">The target connection string builder.</param>
    /// <param name="compareDatabase">Flag indicating whether the database parameter is considered in the comparison.</param>
    /// <returns><c>true</c> if the two connection strings are similar in their core host parameters, <c>false</c> otherwise.</returns>
    public static bool CompareHostParameters(this DbConnectionStringBuilder sourceConnectionStringBuilder, DbConnectionStringBuilder targetConnectionStringBuilder, bool compareDatabase)
    {
      if (sourceConnectionStringBuilder == null && targetConnectionStringBuilder == null)
      {
        return true;
      }

      if (sourceConnectionStringBuilder == null || targetConnectionStringBuilder == null)
      {
        return false;
      }

      bool areSimilar = sourceConnectionStringBuilder.ContainsKey("server") && targetConnectionStringBuilder.ContainsKey("server");
      areSimilar = areSimilar && sourceConnectionStringBuilder["server"].ToString().Equals(targetConnectionStringBuilder["server"].ToString(), StringComparison.InvariantCultureIgnoreCase);
      areSimilar = areSimilar && sourceConnectionStringBuilder.ContainsKey("port") && targetConnectionStringBuilder.ContainsKey("port");
      areSimilar = areSimilar && sourceConnectionStringBuilder["port"].ToString().Equals(targetConnectionStringBuilder["port"].ToString(), StringComparison.InvariantCultureIgnoreCase);
      areSimilar = areSimilar && sourceConnectionStringBuilder.ContainsKey("user id") && targetConnectionStringBuilder.ContainsKey("user id");
      areSimilar = areSimilar && sourceConnectionStringBuilder["user id"].ToString().Equals(targetConnectionStringBuilder["user id"].ToString(), StringComparison.InvariantCultureIgnoreCase);
      if (compareDatabase)
      {
        areSimilar = areSimilar && sourceConnectionStringBuilder.ContainsKey("database") && targetConnectionStringBuilder.ContainsKey("database");
        areSimilar = areSimilar && sourceConnectionStringBuilder["database"].ToString().Equals(targetConnectionStringBuilder["database"].ToString(), StringComparison.InvariantCultureIgnoreCase);
      }

      return areSimilar;
    }

    /// <summary>
    /// Compares two connection strings to see if they are similar by checking their core host parameters (server, port, user id and database).
    /// </summary>
    /// <param name="sourceConnectionString">The source connection string.</param>
    /// <param name="targetConnectionString">The target connection string.</param>
    /// <param name="compareDatabase">Flag indicating whether the database parameter is considered in the comparison.</param>
    /// <returns><c>true</c> if the two connection strings are similar in their core host parameters, <c>false</c> otherwise.</returns>
    public static bool CompareHostParameters(this string sourceConnectionString, string targetConnectionString, bool compareDatabase)
    {
      if (string.IsNullOrEmpty(sourceConnectionString) && string.IsNullOrEmpty(targetConnectionString))
      {
        return true;
      }

      if (string.IsNullOrEmpty(sourceConnectionString) || string.IsNullOrEmpty(targetConnectionString))
      {
        return false;
      }

      var sourceConnectionSb = new DbConnectionStringBuilder { ConnectionString = sourceConnectionString };
      var targetConnectionSb = new DbConnectionStringBuilder { ConnectionString = targetConnectionString };
      return sourceConnectionSb.CompareHostParameters(targetConnectionSb, compareDatabase);
    }

    /// <summary>
    /// Returns a value indicating whether the specified <see cref="String"/> object occurs within the given source string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="value">The string to seek. </param>
    /// <param name="stringComparison">A <see cref="StringComparison"/> value.</param>
    /// <returns><c>true</c> if the value parameter occurs within this string, or if value is the empty string; otherwise, <c>false</c>.</returns>
    public static bool Contains(this string source, string value, StringComparison stringComparison)
    {
      return !string.IsNullOrEmpty(source) && (string.IsNullOrEmpty(value) || source.IndexOf(value, stringComparison) >= 0);
    }

    /// <summary>
    /// Creates a cursor from a bitmap image.
    /// </summary>
    /// <param name="bmp">Base image for the cursor.</param>
    /// <param name="xHotSpot">The x-coordinate of a cursor's hot spot (normally the upper-left corner of the cursor).</param>
    /// <param name="yHotSpot">The y-coordinate of a cursor's hot spot (normally the upper-left corner of the cursor).</param>
    /// <returns>The cursor created from the given bitmap.</returns>
    public static Cursor CreateCursor(this Bitmap bmp, int xHotSpot, int yHotSpot)
    {
      if (bmp == null)
      {
        return null;
      }

      var tmp = new IconInfo();
      GetIconInfo(bmp.GetHicon(), ref tmp);
      tmp.xHotspot = xHotSpot;
      tmp.yHotspot = yHotSpot;
      tmp.fIcon = false;
      return new Cursor(CreateIconIndirect(ref tmp));
    }

    /// <summary>
    /// Compares the elements of two arrays of strings for equality.
    /// </summary>
    /// <param name="sourceArray">The source array.</param>
    /// <param name="targetArray">The target array.</param>
    /// <returns><c>true</c> if the elements of both arrays are equal, <c>false</c> otherwise.</returns>
    public static bool ElementsAreEqual(this string[] sourceArray, string[] targetArray)
    {
      if (sourceArray == null && targetArray == null)
      {
        return true;
      }

      if (sourceArray == null || targetArray == null)
      {
        return false;
      }

      return sourceArray.SequenceEqual(targetArray);
    }

    /// <summary>
    /// Gets a property's alternate name (if the property is decorated by the <see cref="AlternateNameAttribute"/>).
    /// </summary>
    /// <param name="propertyInfo">A <see cref="PropertyInfo"/> instance.</param>
    /// <returns>An alternate name for the given property if applicable.</returns>
    public static string GetAlternateName(this PropertyInfo propertyInfo)
    {
      if (propertyInfo == null)
      {
        return null;
      }

      return propertyInfo.GetCustomAttributes(false).FirstOrDefault(a => a is AlternateNameAttribute) is AlternateNameAttribute alternateAttribute
        ? alternateAttribute.Name
        : null;
    }

    /// <summary>
    /// Gets a property's alternate name (if the property is decorated by the <see cref="AlternateNameAttribute"/>).
    /// </summary>
    /// <param name="instance">An object instance.</param>
    /// <param name="propertyName">A property name in the object's class.</param>
    /// <param name="bindingFlags">The <see cref="BindingFlags"/> to match a property.</param>
    /// <returns>An alternate name for the given property if applicable.</returns>
    public static string GetAlternateName(this object instance, string propertyName, BindingFlags bindingFlags)
    {
      if (instance == null
          || string.IsNullOrEmpty(propertyName))
      {
        return null;
      }

      var propertyInfo = instance.GetType().GetProperty(propertyName, bindingFlags);
      return propertyInfo.GetAlternateName();
    }

    /// <summary>
    /// Returns a control with the given name that is a direct or indirect child of the given control.
    /// </summary>
    /// <param name="control">A <see cref="Control"/> instance.</param>
    /// <param name="name">The name of the control to match.</param>
    /// <returns>A control with the given name that is a direct or indirect child of the given control.</returns>
    public static Control GetChildControl(this Control control, string name)
    {
      if (control == null)
      {
        return null;
      }

      if (control.Controls.Count == 0)
      {
        return null;
      }

      foreach (Control c in control.Controls)
      {
        if (c.Name.Equals(name, StringComparison.Ordinal))
        {
          return c;
        }
      }

      return control.Controls.Cast<Control>().Select(c => GetChildControl(c, name)).FirstOrDefault(foundControl => foundControl != null);
    }

    /// <summary>
    /// Gets recursively child controls of the given type.
    /// </summary>
    /// <param name="control">A <see cref="Control"/> instance.</param>
    /// <returns>Child controls of the given type.</returns>
    public static IEnumerable<T> GetChildControlsOfType<T>(this Control control)
    {
      if (control == null)
      {
        return null;
      }

      var controls = control.Controls.Cast<Control>();
      var enumerable = controls as IList<Control> ?? controls.ToList();
      return enumerable
          .OfType<T>()
          .Concat(enumerable.SelectMany(GetChildControlsOfType<T>));
    }

    /// <summary>
    /// Returns a tuple with the class ID and value of an informational item returned by the X Protocol.
    /// </summary>
    /// <param name="xString">An informational item returned by the X Protocol.</param>
    /// <returns>A tuple with the class ID and value of an informational item returned by the X Protocol.</returns>
    public static Tuple<string, string> GetClassIdAndValue(this string xString)
    {
      if (string.IsNullOrEmpty(xString))
      {
        return null;
      }

      var match = ClassIdAndValueRegex.Match(xString);

      // Note that Groups[0] contains the whole matched string, so we want the sub-groups in Groups[1] and Groups[2]
      return match.Success
        ? new Tuple<string, string>(match.Groups[1].Value, match.Groups[2].Value)
        : null;
    }

    /// <summary>
    /// Gets a list of all MySQL character sets along with their available collations.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="firstElement">A custom string for the first element of the dictionary.</param>
    /// <returns>A list of all MySQL character sets along with their available collations.</returns>
    public static Dictionary<string, string[]> GetCollationsDictionary(this MySqlConnection connection, string firstElement = null)
    {
      return Utilities.GetCollationsDictionary(connection?.ConnectionString, firstElement);
    }

    /// <summary>
    /// Gets a description for a given <see cref="MySqlConnectionProtocol"/> value corresponding to a <see cref="MySqlWorkbenchConnection.ConnectionMethodType"/> one.
    /// </summary>
    /// <param name="connectionProtocol"> A <see cref="MySqlConnectionProtocol"/> value.</param>
    /// <returns>A description for a given <see cref="MySqlConnectionProtocol"/> value corresponding to a <see cref="MySqlWorkbenchConnection.ConnectionMethodType"/> one.</returns>
    public static string GetConnectionProtocolDescription(this MySqlConnectionProtocol connectionProtocol)
    {
      return (int) connectionProtocol < 2
        ? MySqlWorkbenchConnection.ConnectionMethodType.Tcp.GetDescription()
        : MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe.GetDescription();
    }

    /// <summary>
    /// Gets the default collation corresponding to the given character set name.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="charSet"></param>
    /// <returns>Tthe default collation corresponding to the given character set name.</returns>
    public static string GetDefaultCollationFromCharSet(this MySqlConnection connection, string charSet)
    {
      return Utilities.GetDefaultCollationFromCharSet(connection?.ConnectionString, charSet);
    }

    /// <summary>
    /// Gets the text defined in the Description attribute of an enumeration value.
    /// </summary>
    /// <param name="value">An enumeration value.</param>
    /// <returns>The text defined in the Description attribute of an enumeration value, if not defined it returns the value converted to string.</returns>
    public static string GetDescription(this Enum value)
    {
      var field = value.GetType().GetField(value.ToString());
      var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
      return attribute == null ? value.ToString() : attribute.Description;
    }

    /// <summary>
    /// Gets a dictionary containing keys representing enumeration values, and values representing enumeration descriptions.
    /// </summary>
    /// <param name="value">An enumeration value.</param>
    /// <param name="splitEnumValuesByCaps">Flag indicating whether enumeration values are split with spaces before a capital letter is found.</param>
    /// <param name="repeatKeyInValue">Flag indicating whether the key text is prepended to the value text to produce something like "keyText - valueText".</param>
    /// <param name="skipGivenValue">Flag indicating whether the given enumeration value should not be included in the dictionary.</param>
    /// <param name="stripTextInValue">Text to strip from the enumeration value, if <c>null</c> the enumeration value is used as is.</param>
    /// <returns>A dictionary containing keys representing enumeration values, and values representing enumeration descriptions.</returns>
    public static Dictionary<string, string> GetDescriptionsDictionary(this Enum value, bool splitEnumValuesByCaps, bool repeatKeyInValue, bool skipGivenValue, string stripTextInValue = null)
    {
      var enumerationValues = Enum.GetValues(value.GetType());
      var dictionary = new Dictionary<string, string>(enumerationValues.Length);
      foreach (Enum enumValue in enumerationValues)
      {
        if (skipGivenValue && Equals(enumValue, value))
        {
          continue;
        }

        var keyText = enumValue.ToString();
        if (!string.IsNullOrEmpty(stripTextInValue))
        {
          keyText = keyText.Replace(stripTextInValue, string.Empty);
        }

        if (splitEnumValuesByCaps)
        {
          keyText = Regex.Replace(keyText, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        var valueText = repeatKeyInValue
          ? keyText + " - " + enumValue.GetDescription()
          : keyText;
        dictionary.Add(keyText, valueText);
      }

      return dictionary;
    }

    /// <summary>
    /// Gets a collection of flags declared in an enumeration if any.
    /// </summary>
    /// <param name="enumeration">An enumeration value.</param>
    /// <returns>A collection of flags declared in an enumeration if any.</returns>
    public static IEnumerable<Enum> GetFlags(this Enum enumeration)
    {
      return Enum.GetValues(enumeration.GetType()).Cast<Enum>().Where(enumeration.HasFlag);
    }

    /// <summary>
    /// Gets a formatted string with the messages in the given <see cref="Exception"/> and its <see cref="Exception.InnerException"/> if it exists.
    /// </summary>
    /// <param name="exception">An <see cref="Exception"/> object.</param>
    /// <returns>A formatted string with the messages in the given <see cref="Exception"/> and its <see cref="Exception.InnerException"/> if it exists.</returns>
    public static string GetFormattedMessage(this Exception exception)
    {
      if (exception == null)
      {
        return string.Empty;
      }

      var formattedMessagesBuilder = new StringBuilder(exception.Message);
      if (exception.InnerException != null)
      {
        formattedMessagesBuilder.Append(Environment.NewLine);
        formattedMessagesBuilder.Append(Environment.NewLine);
        formattedMessagesBuilder.Append(exception.InnerException.Message);
      }

      return formattedMessagesBuilder.ToString();
    }

    /// <summary>
    /// Builds the host identifier describing where the MySQL server instance can be reached at.
    /// </summary>
    /// <param name="stringBuilder">A <see cref="MySqlConnectionStringBuilder"/> instance.</param>
    /// <returns>The host identifier describing where the MySQL server instance can be reached at.</returns>
    public static string GetHostIdentifier(this MySqlConnectionStringBuilder stringBuilder)
    {
      if (stringBuilder == null)
      {
        return string.Empty;
      }

      switch (stringBuilder.ConnectionProtocol)
      {
        case MySqlConnectionProtocol.UnixSocket:
        case MySqlConnectionProtocol.NamedPipe:
          return $"{MySqlWorkbenchConnection.DEFAULT_DATABASE_DRIVER_NAME}@local:{stringBuilder.PipeName}";

        default:
          return $"{MySqlWorkbenchConnection.DEFAULT_DATABASE_DRIVER_NAME}@{stringBuilder.Server}:{stringBuilder.Port}";
      }
    }

    /// <summary>
    /// Gets the length, in pixels, of the longest key or value element among the specified dictionary.
    /// </summary>
    /// <param name="dictionary">A dictionary of string tuples.</param>
    /// <param name="useKey">Flag indicating whether the key (first) element of each tuple to calculate the lenght, or the value (second) element.</param>
    /// <param name="font">The <see cref="Font"/> used to draw the text.</param>
    /// <param name="addedPadding">Length, in pixels, of any padding to add to the computed length.</param>
    /// <returns>The length, in pixels, of the longest key or value element among the specified dictionary.</returns>
    public static int GetMaxElementLength(this Dictionary<string, string> dictionary, bool useKey, Font font, int addedPadding = 0)
    {
      if (dictionary == null)
      {
        return 0;
      }

      int longestLength = 0;
      foreach (var tuple in dictionary)
      {
        longestLength = Math.Max(longestLength, TextRenderer.MeasureText(useKey ? tuple.Key : tuple.Value, font).Width);
      }

      return longestLength + addedPadding;
    }

    /// <summary>
    /// Gets the character set and its collation used by the connected MySQL server.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <returns>The character set and its collation used by the connected MySQL server.</returns>
    public static Tuple<string, string> GetMySqlServerCharSetAndCollation(this MySqlConnection connection)
    {
      return Utilities.GetMySqlServerCharSetAndCollation(connection?.ConnectionString);
    }

    /// <summary>
    /// Gets the value of the DEFAULT_STORAGE_ENGINE MySQL Server variable indicating the default DB engine used for new table creations.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <returns>The default DB engine used for new table creations.</returns>
    public static string GetMySqlServerDefaultEngine(this MySqlConnection connection)
    {
      return Utilities.GetMySqlServerDefaultEngine(connection?.ConnectionString);
    }

    /// <summary>
    /// Gets the value of the global SQL_MODE MySQL Server variable.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <returns>The value of the global SQL_MODE system variable.</returns>
    public static string GetMySqlServerGlobalMode(this MySqlConnection connection)
    {
      return Utilities.GetMySqlServerGlobalMode(connection?.ConnectionString);
    }

    /// <summary>
    /// Gets the value of the LOWER_CASE_TABLE_NAMES MySQL Server variable indicating the case sensitive.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <returns><c>true</c> if table names are stored in lowercase on disk and comparisons are not case sensitive, <c>false</c> if table names are stored as specified and comparisons are case sensitive.</returns>
    public static bool GetMySqlServerLowerCaseTableNames(this MySqlConnection connection)
    {
      return Utilities.GetMySqlServerLowerCaseTableNames(connection?.ConnectionString);
    }

    /// <summary>
    /// Gets the value of the MAX_ALLOWED_PACKET MySQL Server variable indicating the max size in bytes of the packet returned by a single query.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <returns>The max size in bytes of the packet returned by a single query.</returns>
    public static int GetMySqlServerMaxAllowedPacket(this MySqlConnection connection)
    {
      return Utilities.GetMySqlServerMaxAllowedPacket(connection?.ConnectionString);
    }

    /// <summary>
    /// Gets the version of the connected MySQL server.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <returns>The version of the connected MySQL server.</returns>
    public static string GetMySqlServerVersion(this MySqlConnection connection)
    {
      return Utilities.GetMySqlServerVersion(connection?.ConnectionString);
    }

    /// <summary>
    /// Gets a text avoiding duplicates by adding a numeric suffix in case it already exists in the given list.
    /// </summary>
    /// <param name="listOfTexts">The list of texts.</param>
    /// <param name="proposedText">Proposed text.</param>
    /// <returns>Unique text.</returns>
    public static string GetNonDuplicateText(this List<string> listOfTexts, string proposedText)
    {
      if (string.IsNullOrEmpty(proposedText) || listOfTexts == null || listOfTexts.Count == 0)
      {
        return proposedText;
      }

      proposedText = proposedText.Trim();
      string nonDuplicateText = proposedText;
      int textSuffixNumber = 2;
      while (listOfTexts.Exists(text => text == nonDuplicateText))
      {
        nonDuplicateText = proposedText + textSuffixNumber++;
      }

      return nonDuplicateText;
    }

    /// <summary>
    /// Gets a property's numeric code (if the property is decorated by the <see cref="NumericCodeAttribute"/>).
    /// </summary>
    /// <param name="propertyInfo">A <see cref="PropertyInfo"/> instance.</param>
    /// <returns>A numeric code for the given member if applicable.</returns>
    public static int? GetNumericCode(this PropertyInfo propertyInfo)
    {
      if (propertyInfo == null)
      {
        return null;
      }

      if (propertyInfo.GetCustomAttributes(false).FirstOrDefault(a => a is NumericCodeAttribute) is NumericCodeAttribute numericCodeAttribute)
      {
        return numericCodeAttribute.NumericCode;
      }

      return null;
    }

    /// <summary>
    /// Gets the default property value by property name.
    /// </summary>
    /// <typeparam name="T">Type to which the property must be cast to in the end.</typeparam>
    /// <param name="settings">The application settings.</param>
    /// <param name="propertyName">Name of the property we want to get the default value from.</param>
    /// <returns></returns>
    public static T GetPropertyDefaultValueByName<T>(this ApplicationSettingsBase settings, string propertyName)
    {
      var settingsProperty = settings.Properties[propertyName];
      var propertyInfo = settings.GetType().GetProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.InvariantCulture));
      if (propertyInfo == null || settingsProperty == null)
      {
        return default(T);
      }

      return (T)Convert.ChangeType(settingsProperty.DefaultValue, propertyInfo.PropertyType);
    }

    /// <summary>
    /// Gets a list of tuples containing <see cref="PropertyInfo"/>s of properties with matching names or alternate names.
    /// </summary>
    /// <param name="object1">An object.</param>
    /// <param name="object2">Another object.</param>
    /// <param name="useAlternateNames">Flag indicating whether property names are also matched with alternate names given by the <see cref="AlternateNameAttribute"/> decorator.</param>
    /// <param name="exclude">Flag indicating if properties decorated with the <see cref="ExcludeAttribute"/> should be excluded.</param>
    /// <param name="caseSensitive">Flag indicating if property (or alternate property) names are compared case sensitive or insensitive.</param>
    /// <returns>A list of tuples containing <see cref="PropertyInfo"/>s of properties with matching names or alternate names.</returns>
    public static List<Tuple<PropertyInfo, PropertyInfo>> GetPropertyInfosMatching(this object object1, object object2, bool useAlternateNames = true, bool exclude = true, bool caseSensitive = true)
    {
      if (object1 == null)
      {
        throw new ArgumentNullException(nameof(object1));
      }

      if (object2 == null)
      {
        throw new ArgumentNullException(nameof(object2));
      }

      var object1PropertyInfos = object1.GetType().GetProperties();
      var object2PropertyInfos = object2.GetType().GetProperties();
      var stringComparison = caseSensitive
        ? StringComparison.Ordinal
        : StringComparison.OrdinalIgnoreCase;
      var retList = new List<Tuple<PropertyInfo, PropertyInfo>>(Math.Max(object1PropertyInfos.Length, object2PropertyInfos.Length));
      foreach (var object1PropertyInfo in object1PropertyInfos)
      {
        if (!object1PropertyInfo.CanRead
            || exclude && object1PropertyInfo.IsExcluded())
        {
          continue;
        }

        var object1AlternateName = useAlternateNames ? object1PropertyInfo.GetAlternateName() : null;
        var object2PropertyInfo = object2PropertyInfos.FirstOrDefault(o2Pi =>
        {
          var object2AlternateName = useAlternateNames ? o2Pi.GetAlternateName() : null;
          return o2Pi.Name.Equals(object1PropertyInfo.Name, stringComparison)
                 || !string.IsNullOrEmpty(object1AlternateName)
                    && o2Pi.Name.Equals(object1AlternateName, stringComparison)
                 || !string.IsNullOrEmpty(object2AlternateName)
                    && object2AlternateName.Equals(object1PropertyInfo.Name, stringComparison)
                 || !string.IsNullOrEmpty(object2AlternateName)
                    && !string.IsNullOrEmpty(object1AlternateName)
                    && object2AlternateName.Equals(object1AlternateName, stringComparison);
        });
        if (object2PropertyInfo == null
            || exclude && object2PropertyInfo.IsExcluded())
        {
          continue;
        }

        retList.Add(new Tuple<PropertyInfo, PropertyInfo>(object1PropertyInfo, object2PropertyInfo));
      }

      return retList;
    }

    /// <summary>
    /// Gets the character set and its collation used by the currently selected schema.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <returns>The character set and its collation used by the currently selected schema.</returns>
    public static Tuple<string, string> GetSchemaCharSetAndCollation(this MySqlConnection connection, string schemaName)
    {
      return Utilities.GetSchemaCharSetAndCollation(connection?.ConnectionString, schemaName);
    }

    /// <summary>
    /// Gets the schema information ofr the given database collection.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="schemaInformation">The type of schema information to query.</param>
    /// <param name="showErrors">Flag indicating whether errors are shown to the users if any or just logged.</param>
    /// <param name="restrictions">Specific parameters that vary among database collections.</param>
    /// <returns>Schema information within a data table.</returns>
    public static DataTable GetSchemaInformation(this MySqlConnection connection, SchemaInformationType schemaInformation, bool showErrors, params string[] restrictions)
    {
      return Utilities.GetSchemaInformation(connection?.ConnectionString, schemaInformation, showErrors, restrictions);
    }

    /// <summary>
    /// Gets the collation defined on a MySQL table with the given name.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <param name="tableName">The name of the database table.</param>
    /// <returns>The collation defined on a MySQL table with the given name.</returns>
    public static string GetTableCollation(this MySqlConnection connection, string schemaName, string tableName)
    {
      return connection.GetTableCollation(schemaName, tableName, out string _);
    }

    /// <summary>
    /// Gets the collation defined on a MySQL table with the given name.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <param name="tableName">The name of the database table.</param>
    /// <param name="charSet">The character set that belongs to the table collation.</param>
    /// <returns>The collation defined on a MySQL table with the given name.</returns>
    public static string GetTableCollation(this MySqlConnection connection, string schemaName, string tableName, out string charSet)
    {
      return Utilities.GetTableCollation(connection?.ConnectionString, schemaName, tableName, out charSet);
    }

    /// <summary>
    /// Gets a dictionary containing keys representing enumeration values, and values representing enumeration descriptions.
    /// </summary>
    /// <param name="value">An enumeration value.</param>
    /// <param name="skipGivenValue">Flag indicating whether the given enumeration value should not be included in the dictionary.</param>
    /// <param name="skipObsolete">Flag indicating whether enumeration values containing the <see cref="ObsoleteAttribute"/> are not included in the dictionary.</param>
    /// <param name="skipNotSupportedByCaller">
    /// Flag indicating whether enumeration values not containing the <see cref="SupportedByAttribute"/>,
    /// or containing it but the caller's assembly does not contain the <see cref="SupportedByAttribute.Name"/>, are not included in the dictionary.
    /// </param>
    /// <returns>A dictionary containing keys representing enumeration values, and values representing enumeration descriptions.</returns>
    public static Dictionary<Enum, string> GetValuesAndDescriptionsDictionary(this Enum value, bool skipGivenValue = false, bool skipObsolete = true, bool skipNotSupportedByCaller = false)
    {
      var enumerationValues = Enum.GetValues(value.GetType());
      var dictionary = new Dictionary<Enum, string>(enumerationValues.Length);
      foreach (Enum enumValue in enumerationValues)
      {
        if ((skipGivenValue && Equals(enumValue, value))
            || (skipObsolete && enumValue.IsObsolete())
            || (skipNotSupportedByCaller && !enumValue.IsSupportedByCaller()))
        {
          continue;
        }

        dictionary.Add(enumValue, enumValue.GetDescription());
      }

      return dictionary;
    }
    /// <summary>
    /// Gets the width of text drawn on the given control that can fit within its drawing area by splitting the text in lines.
    /// </summary>
    /// <param name="control">The control where we want to draw the text, normally a label.</param>
    /// <param name="maxlinesOfText">The maximum lines in which the text can be split into.</param>
    /// <param name="overridingText">The text to be drawn in the control, if <c>null</c> the control's Text is used.</param>
    /// <returns>The width of the text split into lines.</returns>
    public static int GetWidthOfTextSplitInLines(this Control control, int maxlinesOfText, string overridingText = null)
    {
      int maxWidth = 0;
      if (control == null)
      {
        return maxWidth;
      }

      if (maxlinesOfText <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(maxlinesOfText));
      }

      if (overridingText == null)
      {
        overridingText = control.Text;
      }

      maxWidth = control.PreferredSize.Width;
      if (string.IsNullOrEmpty(overridingText) || maxWidth <= control.Width)
      {
        return control.Width;
      }

      int lines = 1;
      while (lines <= maxlinesOfText && maxWidth > control.Width)
      {
        maxWidth = control.PreferredSize.Width / lines++;
      }

      lines = Math.Min(lines, maxlinesOfText);
      int wordWrappedLines;
      int step = Math.Abs(maxWidth - control.Width);
      int stepMultiplier = 0;
      do
      {
        maxWidth += step * stepMultiplier;
        stepMultiplier = 1;
        wordWrappedLines = control.WordWrapText(overridingText, maxWidth).Count;
      }
      while (wordWrappedLines > lines);

      return maxWidth;
    }

    /// <summary>
    /// Checks if an <see cref="ErrorProvider"/> has set an error on the given control within its parent container.
    /// </summary>
    /// <param name="errorProvider">An <see cref="ErrorProvider"/> instance.</param>
    /// <param name="onControl">A specific <see cref="Control"/> to check onto.</param>
    /// <returns><c>true</c> if the <see cref="ErrorProvider"/> has set an error on the given control within its parent container, <c>false</c> otherwise.</returns>
    public static bool HasErrors(this ErrorProvider errorProvider, Control onControl)
    {
      return errorProvider != null
             && errorProvider.ContainerControl.GetChildControlsOfType<Control>().Any(c => c == onControl && !string.IsNullOrEmpty(errorProvider.GetError(c)));
    }

    /// <summary>
    /// Checks if an <see cref="ErrorProvider"/> has set an error on any control within its parent container.
    /// </summary>
    /// <param name="errorProvider">An <see cref="ErrorProvider"/> instance.</param>
    /// <returns><c>true</c> if the <see cref="ErrorProvider"/> has set an error on any control within its parent container, <c>false</c> otherwise.</returns>
    public static bool HasErrors(this ErrorProvider errorProvider)
    {
      return errorProvider != null
             && errorProvider.ContainerControl.GetChildControlsOfType<Control>().Any(c => !string.IsNullOrEmpty(errorProvider.GetError(c)));
    }

    /// <summary>
    /// Checks if two objects have the same property values on their matching property names or alternate names.
    /// </summary>
    /// <param name="object1">An object.</param>
    /// <param name="object2">Another object.</param>
    /// <param name="useAlternateNames">Flag indicating whether property names are also matched with alternate names given by the <see cref="AlternateNameAttribute"/> decorator.</param>
    /// <param name="exclude">Flag indicating if properties decorated with the <see cref="ExcludeAttribute"/> should be excluded.</param>
    /// <param name="caseSensitive">Flag indicating if property (or alternate property) names are compared case sensitive or insensitive.</param>
    /// <returns><c>true</c> if the two objects have the same property values on their matching property names or alternate names, <c>false</c> otherwise.</returns>
    public static bool HasSamePropertyValues(this object object1, object object2, bool useAlternateNames = true, bool exclude = true, bool caseSensitive = true)
    {
      var propertyInfoTuples = object1.GetPropertyInfosMatching(object2, useAlternateNames, exclude, caseSensitive);
      return propertyInfoTuples.All(tuple => tuple.Item1.GetValue(object1) == tuple.Item2.GetValue(object2));
    }

    /// <summary>
    /// Initializes the given <see cref="ComboBox"/> with values and descriptions in an enumeration given an enumeration value.
    /// </summary>
    /// <param name="comboBox">A <see cref="ComboBox"/> instance.</param>
    /// <param name="enumerationValue">An enumeration value. Note that this enumeration value</param>
    /// <param name="skipGivenValue">Flag indicating whether the given enumeration value is not included in the dictionary.</param>
    /// <param name="skipObsolete">Flag indicating whether enumeration values flagged as obsolete are not included in the dictionary.</param>
    /// <param name="skipNotSupportedByCaller">
    /// Flag indicating whether enumeration values not containing the <see cref="SupportedByAttribute"/>,
    /// or containing it but the caller's assembly does not contain the <see cref="SupportedByAttribute.Name"/>, are not included in the dictionary.
    /// </param>
    public static void InitializeComboBoxFromEnum(this ComboBox comboBox, Enum enumerationValue, bool skipGivenValue = false, bool skipObsolete = true, bool skipNotSupportedByCaller = false)
    {
      if (comboBox == null)
      {
        return;
      }

      comboBox.DisplayMember = "Value";
      comboBox.ValueMember = "Key";
      comboBox.DataSource = new BindingSource(enumerationValue.GetValuesAndDescriptionsDictionary(skipGivenValue, skipObsolete, skipNotSupportedByCaller), null);
    }

    /// <summary>
    /// Checks if the given <see cref="CommandAreaProperties.ButtonsLayoutType"/> value corresponds to a layout with 2 buttons.
    /// </summary>
    /// <param name="layoutType">A CommandAreaProperties.ButtonsLayoutType value.</param>
    /// <returns><c>true</c> if the given <see cref="CommandAreaProperties.ButtonsLayoutType"/> value corresponds to a layout with 2 buttons, <c>false</c> otherwise.</returns>
    public static bool Is2Button(this CommandAreaProperties.ButtonsLayoutType layoutType)
    {
      return layoutType == CommandAreaProperties.ButtonsLayoutType.Generic2Buttons
                            || layoutType == CommandAreaProperties.ButtonsLayoutType.OkCancel
                            || layoutType == CommandAreaProperties.ButtonsLayoutType.YesNo;
    }

    /// <summary>
    /// Checks if the given <see cref="CommandAreaProperties.ButtonsLayoutType"/> value corresponds to a layout with 3  buttons.
    /// </summary>
    /// <param name="layoutType">A CommandAreaProperties.ButtonsLayoutType value.</param>
    /// <returns><c>true</c> if the given <see cref="CommandAreaProperties.ButtonsLayoutType"/> value corresponds to a layout with 3 buttons, <c>false</c> otherwise.</returns>
    public static bool Is3Button(this CommandAreaProperties.ButtonsLayoutType layoutType)
    {
      return layoutType == CommandAreaProperties.ButtonsLayoutType.Generic3Buttons
                            || layoutType == CommandAreaProperties.ButtonsLayoutType.YesNoCancel;
    }

    /// <summary>
    /// Checks if the given text result is a JSON collection in string representation.
    /// </summary>
    /// <param name="stringResult">A text.</param>
    /// <returns><c>true</c> if the given text result is a JSON collection in string representation, <c>false</c> otherwise.</returns>
    public static bool IsCollection(this string stringResult)
    {
      return !string.IsNullOrEmpty(stringResult)
             && stringResult.StartsWith("[", StringComparison.Ordinal)
             && stringResult.EndsWith("]", StringComparison.Ordinal);
    }

    /// <summary>
    /// Checks if a property is excluded from operations (decorated by the <see cref="ExcludeAttribute"/>).
    /// </summary>
    /// <param name="propertyInfo">A <see cref="PropertyInfo"/> instance.</param>
    /// <returns><c>true</c> if a property is excluded from operations (decorated by the <see cref="ExcludeAttribute"/>), <c>false</c> otherwise.</returns>
    public static bool IsExcluded(this PropertyInfo propertyInfo)
    {
      if (propertyInfo == null)
      {
        throw new ArgumentNullException(nameof(propertyInfo));
      }

      return propertyInfo.GetCustomAttributes(false).Any(a => a is ExcludeAttribute);
    }

    /// <summary>
    /// Checks if a property is excluded from operations (decorated by the <see cref="ExcludeAttribute"/>).
    /// </summary>
    /// <param name="instance">An object instance.</param>
    /// <param name="propertyName">A property name in the object's class.</param>
    /// <param name="bindingFlags">The <see cref="BindingFlags"/> to match a property.</param>
    /// <returns><c>true</c> if a property is excluded from operations (decorated by the <see cref="ExcludeAttribute"/>), <c>false</c> otherwise.</returns>
    public static bool IsExcluded(this object instance, string propertyName, BindingFlags bindingFlags)
    {
      var propertyInfo = instance.GetType().GetProperty(propertyName, bindingFlags);
      return propertyInfo.IsExcluded();
    }

    /// <summary>
    /// Checks if the given <see cref="CommandAreaProperties.ButtonsLayoutType"/> value corresponds to a generic layout.
    /// </summary>
    /// <param name="layoutType">A CommandAreaProperties.ButtonsLayoutType value.</param>
    /// <returns><c>true</c> if the given <see cref="CommandAreaProperties.ButtonsLayoutType"/> value corresponds to a generic layout, <c>false</c> otherwise.</returns>
    public static bool IsGeneric(this CommandAreaProperties.ButtonsLayoutType layoutType)
    {
      return layoutType == CommandAreaProperties.ButtonsLayoutType.Generic1Button
                            || layoutType == CommandAreaProperties.ButtonsLayoutType.Generic2Buttons
                            || layoutType == CommandAreaProperties.ButtonsLayoutType.Generic3Buttons;
    }

    /// <summary>
    /// Checks if the given enumeration value has been flagged with the <see cref="ObsoleteAttribute"/>.
    /// </summary>
    /// <param name="value">An enumeration value.</param>
    /// <returns><c>true</c> if the given enumeration value has been flagged with the <see cref="ObsoleteAttribute"/>, <c>false</c> otherwise.</returns>
    public static bool IsObsolete(this Enum value)
    {
      var field = value.GetType().GetField(value.ToString());
      var attribute = Attribute.GetCustomAttribute(field, typeof(ObsoleteAttribute)) as ObsoleteAttribute;
      return attribute != null;
    }

    /// <summary>
    /// Checks if 2 double values are practically equal.
    /// </summary>
    /// <param name="value1">The first double value.</param>
    /// <param name="value2">The second double value.</param>
    /// <param name="units">The difference between the integer representation of two floating-point values.</param>
    /// <returns><c>true</c> if both double values are practically equal, <c>false</c> otherwise.</returns>
    public static bool IsPracticallyEqual(this double value1, double value2, int units = 1)
    {
      if (double.IsNaN(value1) && double.IsNaN(value2)
          || double.IsNegativeInfinity(value1) && double.IsNegativeInfinity(value2)
          || double.IsPositiveInfinity(value1) && double.IsPositiveInfinity(value2))
      {
        return true;
      }

      long lValue1 = BitConverter.DoubleToInt64Bits(value1);
      long lValue2 = BitConverter.DoubleToInt64Bits(value2);

      // If the signs are different, return false except for +0 and -0.
      if (lValue1 >> 63 != lValue2 >> 63)
      {
        return Math.Abs(value1 - value2) < double.Epsilon;
      }

      long diff = Math.Abs(lValue1 - lValue2);
      return diff <= units;
    }

    /// <summary>
    /// Checks if the given enumeration value contains the <see cref="SupportedByAttribute"/> and the caller's assembly contains the <see cref="SupportedByAttribute.Name"/>.
    /// </summary>
    /// <param name="value">An enumeration value.</param>
    /// <returns>
    /// <c>true</c> if the given enumeration value contains the <see cref="SupportedByAttribute"/> and if the <see cref="SupportedByAttribute.Name"/> is "*" or "ALL",
    /// or if the caller's assembly contains the <see cref="SupportedByAttribute.Name"/>, <c>false</c> otherwise.
    /// </returns>
    public static bool IsSupportedByCaller(this Enum value)
    {
      var field = value.GetType().GetField(value.ToString());
      var attribute = Attribute.GetCustomAttribute(field, typeof(SupportedByAttribute)) as SupportedByAttribute;
      if (attribute == null || string.IsNullOrEmpty(attribute.Name))
      {
        return false;
      }

      if (attribute.Name == "*" || attribute.Name.Equals("all", StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      return Assembly.GetCallingAssembly().EscapedCodeBase.Contains(attribute.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Normalizes the new line characters in the provided string by replacing lone '\n' with Environment.NewLine.
    /// </summary>
    /// <param name="text">The text to normalize</param>
    /// <returns>The normalized text without lone '\n' characters.</returns>
    public static string NormalizeNewLineCharacters(this string text)
    {
      if (text == null)
      {
        return null;
      }

      var myRegEx = new Regex(@"(?<!\r)\n");
      return myRegEx.Replace(text, Environment.NewLine);
    }

    /// <summary>
    /// Reads an 8-byte floating point (<see cref="double"/>) value from the given <see cref="BinaryReader"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    /// <returns>An 8-byte floating point value (<see cref="double"/>).</returns>
    public static double ReadDoubleUsingByteOrder(this BinaryReader reader, ByteOrderType byteOrder)
    {
      if (reader == null)
      {
        return double.NaN;
      }

      var doubleValue = reader.ReadDouble();
      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        return doubleValue;
      }

      return doubleValue.ReverseBytes();
    }

    /// <summary>
    /// Reads a 4-byte signed integer (<see cref="int"/>) value from the given <see cref="BinaryReader"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    /// <returns>An 4-byte signed integer (<see cref="int"/>).</returns>
    public static int ReadIntUsingByteOrder(this BinaryReader reader, ByteOrderType byteOrder)
    {
      if (reader == null)
      {
        return int.MinValue;
      }

      var intValue = reader.ReadInt32();
      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        return intValue;
      }

      return intValue.ReverseBytes();
    }

    /// <summary>
    /// Reads an 8-byte signed integer (<see cref="long"/>) value from the given <see cref="BinaryReader"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    /// <returns>An 8-byte signed integer (<see cref="long"/>).</returns>
    public static long ReadLongUsingByteOrder(this BinaryReader reader, ByteOrderType byteOrder)
    {
      if (reader == null)
      {
        return long.MinValue;
      }

      var longValue = reader.ReadInt64();
      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        return longValue;
      }

      return longValue.ReverseBytes();
    }

    /// <summary>
    /// Reads a 4-byte unsigned integer (<see cref="uint"/>) value from the given <see cref="BinaryReader"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    /// <returns>A 4-byte unsigned integer (<see cref="uint"/>).</returns>
    public static uint ReadUIntUsingByteOrder(this BinaryReader reader, ByteOrderType byteOrder)
    {
      if (reader == null)
      {
        return uint.MinValue;
      }

      var uintValue = reader.ReadUInt32();
      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        return uintValue;
      }

      return uintValue.ReverseBytes();
    }

    /// <summary>
    /// Resizes an <see cref="Icon"/>.
    /// </summary>
    /// <param name="icon">An <see cref="Icon"/> instance.</param>
    /// <param name="size">The new size for the icon.</param>
    /// <returns>A new resized <see cref="Icon"/>.</returns>
    public static Icon Resize(this Icon icon, Size size)
    {
      if (icon == null)
      {
        return null;
      }

      var bitmap = new Bitmap(size.Width, size.Height);
      using (var graphics = Graphics.FromImage(bitmap))
      {
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(icon.ToBitmap(), new Rectangle(Point.Empty, size));
      }

      return Icon.FromHandle(bitmap.GetHicon());
    }

    /// <summary>
    /// Reads an 8-byte unsigned integer (<see cref="ulong"/>) value from the given <see cref="BinaryReader"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    /// <returns>An 8-byte unsigned integer (<see cref="ulong"/>).</returns>
    public static ulong ReadULongUsingByteOrder(this BinaryReader reader, ByteOrderType byteOrder)
    {
      if (reader == null)
      {
        return ulong.MinValue;
      }

      var ulongValue = reader.ReadUInt64();
      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        return ulongValue;
      }

      return ulongValue.ReverseBytes();
    }

    /// <summary>
    /// Reverses the order of the bytes in the given <see cref="double"/> value and returns its corresponding numerical representation.
    /// </summary>
    /// <param name="value">A <see cref="double"/> value.</param>
    /// <returns>A <see cref="double"/> value with its bytes reversed.</returns>
    public static double ReverseBytes(this double value)
    {
      var doubleBytes = BitConverter.GetBytes(value);
      Array.Reverse(doubleBytes);
      return BitConverter.ToDouble(doubleBytes, 0);
    }

    /// <summary>
    /// Reverses the order of the bytes in the given <see cref="int"/> value and returns its corresponding numerical representation.
    /// </summary>
    /// <param name="value">A <see cref="int"/> value.</param>
    /// <returns>A <see cref="int"/> value with its bytes reversed.</returns>
    public static int ReverseBytes(this int value)
    {
      var intBytes = BitConverter.GetBytes(value);
      Array.Reverse(intBytes);
      return BitConverter.ToInt32(intBytes, 0);
    }

    /// <summary>
    /// Reverses the order of the bytes in the given <see cref="uint"/> value and returns its corresponding numerical representation.
    /// </summary>
    /// <param name="value">A <see cref="uint"/> value.</param>
    /// <returns>A <see cref="uint"/> value with its bytes reversed.</returns>
    public static uint ReverseBytes(this uint value)
    {
      var uintBytes = BitConverter.GetBytes(value);
      Array.Reverse(uintBytes);
      return BitConverter.ToUInt32(uintBytes, 0);
    }

    /// <summary>
    /// Reverses the order of the bytes in the given <see cref="long"/> value and returns its corresponding numerical representation.
    /// </summary>
    /// <param name="value">A <see cref="long"/> value.</param>
    /// <returns>A <see cref="long"/> value with its bytes reversed.</returns>
    public static long ReverseBytes(this long value)
    {
      var longBytes = BitConverter.GetBytes(value);
      Array.Reverse(longBytes);
      return BitConverter.ToInt64(longBytes, 0);
    }

    /// <summary>
    /// Reverses the order of the bytes in the given <see cref="ulong"/> value and returns its corresponding numerical representation.
    /// </summary>
    /// <param name="value">A <see cref="ulong"/> value.</param>
    /// <returns>A <see cref="ulong"/> value with its bytes reversed.</returns>
    public static ulong ReverseBytes(this ulong value)
    {
      var ulongBytes = BitConverter.GetBytes(value);
      Array.Reverse(ulongBytes);
      return BitConverter.ToUInt64(ulongBytes, 0);
    }

    /// <summary>
    /// Returns an estimated rough size of an <see cref="object"/> instance.
    /// </summary>
    /// <param name="instance">An <see cref="object"/> instance.</param>
    /// <returns>An estimated rough size of an <see cref="object"/> instance.</returns>
    public static int RoughSizeOf(this object instance)
    {
      if (instance == null)
      {
        return 0;
      }

      var typeHandle = instance.GetType().TypeHandle;
      return Marshal.ReadInt32(typeHandle.Value, 4);
    }

    /// <summary>
    /// Escapes characters that require it to fit as part of a MySQL command
    /// </summary>
    /// <param name="entry">Raw user entry value</param>
    /// <param name="exceptionCharacters">Characters to avoid escaping.</param>
    /// <returns>MySQL escaped user entry value</returns>
    public static string Sanitize(this string entry, params char[] exceptionCharacters)
    {
      if (string.IsNullOrEmpty(entry))
      {
        // Nothing to sanitize
        return entry;
      }

      // Exception flags
      var escapeTab = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('\t');
      var escapeCarriageReturn = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('\r');
      var escapeLineFeed = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('\n');
      var escapeZeroChar = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('\0');
      var escapeSingleQuote = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('\'');
      var escapeDoubleQuote = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('"');
      var escapeBackslash = exceptionCharacters.Length == 0 || !exceptionCharacters.Contains('\\');

      var sb = new StringBuilder();
      var entryCharArray = entry.ToCharArray();
      foreach (char c in entryCharArray)
      {
        if (c.Equals('\t')
            && escapeTab)
        {
          sb.Append(@"\t");
          continue;
        }

        if (c.Equals('\r')
            && escapeCarriageReturn)
        {
          sb.Append(@"\r");
          continue;
        }

        if (c.Equals('\n')
            && escapeLineFeed)
        {
          sb.Append(@"\n");
          continue;
        }

        if (c.Equals('\0')
            && escapeZeroChar)
        {
          sb.Append(@"\0");
          continue;
        }

        if ((c.ToString() == @"'" && escapeSingleQuote)
            || (c.ToString() == @"""" && escapeDoubleQuote)
            || (c.ToString() == @"\" && escapeBackslash))
        {
          sb.Append(@"\");
        }

        sb.Append(c);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Sets the net_write_timeout and net_read_timeout MySQL server variables to the given value for the duration of the current client session.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="timeoutInSeconds">The number of seconds to wait for more data from a connection before aborting the read or for a block to be written to a connection before aborting the write.</param>
    public static void SetClientSessionReadWriteTimeouts(MySqlConnection connection, uint timeoutInSeconds)
    {
      Utilities.SetClientSessionReadWriteTimeouts(connection?.ConnectionString, timeoutInSeconds);
    }

    /// <summary>
    /// Sets the DoubleBuffered property of the control to <c>true</c>.
    /// </summary>
    /// <param name="control">A <see cref="Control"/> object.</param>
    public static void SetDoubleBuffered(this Control control)
    {
      if (SystemInformation.TerminalServerSession || control == null)
      {
        return;
      }

      var aProp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
      aProp?.SetValue(control, true, null);
    }

    /// <summary>
    /// Sets properties related to an error provider in a single call.
    /// </summary>
    /// <param name="errorProvider">An <see cref="ErrorProvider"/> instance (can't be <c>null</c>).</param>
    /// <param name="onControl">A <see cref="Control"/> instance (can't be <c>null</c>).</param>
    /// <param name="properties">The properties to apply to the <see cref="ErrorProvider"/>. If <c>null</c> the <see cref="ErrorProviderProperties.Empty"/> properties are used.</param>
    public static void SetProperties(this ErrorProvider errorProvider, Control onControl, ErrorProviderProperties properties)
    {
      if (errorProvider == null)
      {
        throw new ArgumentNullException(nameof(errorProvider));
      }

      if (properties == null)
      {
        properties = ErrorProviderProperties.Empty;
      }

      if (onControl == null)
      {
        throw new ArgumentNullException(nameof(onControl));
      }

      if (properties.Clear)
      {
        errorProvider.Clear();
      }

      var icon = properties.ErrorIcon ?? ErrorProviderDefaultIcon;
      if (errorProvider.Icon != icon)
      {
        errorProvider.Icon = icon;
      }

      errorProvider.SetError(onControl, properties.ErrorMessage);
      if (properties.IconPadding != 0
          && !string.IsNullOrEmpty(properties.ErrorMessage))
      {
        // The icon will  not be displayed with a null or empty text, so no need to set the padding.
        errorProvider.SetIconPadding(onControl, properties.IconPadding);
      }

      errorProvider.SetIconAlignment(onControl, properties.IconAlignment);
    }

    /// <summary>
    /// Sets property values from one object to another if they have the same property names or alternate names.
    /// </summary>
    /// <param name="toObject">The object that have its property values set.</param>
    /// <param name="fromObject">The object from which property values are copied.</param>
    /// <param name="useAlternateNames">Flag indicating whether property names are also matched with alternate names given by the <see cref="AlternateNameAttribute"/> decorator.</param>
    /// <param name="exclude">Flag indicating if properties decorated with the <see cref="ExcludeAttribute"/> should be excluded.</param>
    /// <param name="caseSensitive">Flag indicating if property (or alternate property) names are compared case sensitive or insensitive.</param>
    public static void SetPropertyValuesFrom(this object toObject, object fromObject, bool useAlternateNames = true, bool exclude = true, bool caseSensitive = true)
    {
      var propertyInfoTuples = toObject.GetPropertyInfosMatching(fromObject, useAlternateNames, exclude, caseSensitive);
      foreach (var tuple in propertyInfoTuples)
      {
        var fromObjectPropertyInfo = tuple.Item2;
        var toObjectPropertyInfo = tuple.Item1;
        if (!fromObjectPropertyInfo.CanRead
            || !toObjectPropertyInfo.CanWrite)
        {
          continue;
        }

        toObjectPropertyInfo.SetValue(toObject, fromObjectPropertyInfo.GetValue(fromObject));
      }
    }

    /// <summary>
    /// Sets property values from one object to another if they have the same property names or alternate names.
    /// </summary>
    /// <param name="fromObject">The object from which property values are copied.</param>
    /// <param name="toObject">The object that have its property values set.</param>
    /// <param name="useAlternateNames">Flag indicating whether property names are also matched with alternate names given by the <see cref="AlternateNameAttribute"/> decorator.</param>
    /// <param name="exclude">Flag indicating if properties decorated with the <see cref="ExcludeAttribute"/> should be excluded.</param>
    /// <param name="caseSensitive">Flag indicating if property (or alternate property) names are compared case sensitive or insensitive.</param>
    public static void SetPropertyValuesTo(this object fromObject, object toObject, bool useAlternateNames = true, bool exclude = true, bool caseSensitive = true)
    {
      toObject.SetPropertyValuesFrom(fromObject, useAlternateNames, exclude, caseSensitive);
    }

    /// <summary>
    /// Splits the with tokenizer.
    /// </summary>
    /// <param name="stringToSplit">The string to split.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>List of every token</returns>
    public static IEnumerable<string> SplitWithTokenizer(this string stringToSplit, string delimiter)
    {
      var tokenizer = new BaseTokenizer(stringToSplit) { DelimiterToken = delimiter };
      return tokenizer.BreakIntoStatements().Where(t => !string.IsNullOrEmpty(t));
    }

    /// <summary>
    /// Converts the provided Json string into a boxed result set object similar to the ones previously exposed in the XShell.
    /// </summary>
    /// <param name="jsonString">Json string.</param>
    /// <returns>A boxed <see cref="Result"/>, <see cref="DocResult"/>, <see cref="RowResult"/> or <see cref="SqlResult"/> if the Json string represents a result set. Otherwise it returns a string message.</returns>
    public static object ToBaseShellResultObject(string jsonString)
    {
      if (string.IsNullOrEmpty(jsonString))
      {
        return null;
      }

      var serializer = new JavaScriptSerializer();
      try
      {
        dynamic entity = serializer.Deserialize<object>(jsonString);

        //Determine result set type.
        var type = entity.ContainsKey("affectedItemCount")
          ? BaseShellResultType.Result
          : entity.ContainsKey("documents")
            ? BaseShellResultType.DocResult
            : entity.ContainsKey("rows")
              ? entity.ContainsKey("hasData")
                ? BaseShellResultType.SqlResult
                : BaseShellResultType.RowResult
              : BaseShellResultType.Unknown;

        if (type == BaseShellResultType.Unknown)
        {
          return jsonString;
        }

        //Populate common info for the result set.
        var baseResult = new BaseResult
        {
          ExecutionTime = entity["executionTime"],
          WarningCount = entity["warningCount"],
          Warnings = new List<Dictionary<string, object>>()
        };
        baseResult.Warnings = ToDictionariesList(entity["warnings"]);

        //Populate result set type specific data.
        switch (type)
        {
          case BaseShellResultType.Result:
            var result = new Result
            {
              ExecutionTime = baseResult.ExecutionTime,
              WarningCount = baseResult.WarningCount,
              Warnings = baseResult.Warnings,
              AffectedItemCount = entity["affectedItemCount"],
              AutoIncrementValue = entity["autoIncrementValue"],
              LastDocumentId = entity["lastDocumentId"]
            };
            return result;

          case BaseShellResultType.DocResult:
            var docResult = new DocResult
            {
              ExecutionTime = baseResult.ExecutionTime,
              WarningCount = baseResult.WarningCount,
              Warnings = baseResult.Warnings,
              Documents = ToDictionariesList(entity["documents"])
            };
            return docResult;

          case BaseShellResultType.RowResult:
            var rowResult = new RowResult
            {
              ExecutionTime = baseResult.ExecutionTime,
              WarningCount = baseResult.WarningCount,
              Warnings = baseResult.Warnings,
              Data = ToDictionariesList(entity["rows"])
            };
            return rowResult;

          case BaseShellResultType.SqlResult:
            var sqlResult = new SqlResult
            {
              ExecutionTime = baseResult.ExecutionTime,
              WarningCount = baseResult.WarningCount,
              Warnings = baseResult.Warnings,
              HasData = entity["hasData"],
              AffectedRowCount = entity["affectedRowCount"],
              AutoIncrementValue = entity["autoIncrementValue"],
              Data = ToDictionariesList(entity["rows"])
            };
            return sqlResult;
        }
      }
      catch (Exception)
      {
        //Do nothing. Ignore exception.
      }

      return jsonString;
    }

    /// <summary>
    /// Converts a hexadecimal string to a byte array.
    /// </summary>
    /// <param name="hexValue">A string with a hexadecimal value.</param>
    /// <returns>An array of bytes with the value of the hexadecimal string.</returns>
    public static byte[] ToByteArray(this string hexValue)
    {
      if (string.IsNullOrEmpty(hexValue))
      {
        return null;
      }

      int numChars = hexValue.Length;
      byte[] bytes = new byte[numChars / 2];
      for (int i = 0; i < numChars; i += 2)
      {
        bytes[i / 2] = Convert.ToByte(hexValue.Substring(i, 2), 16);
      }

      return bytes;
    }

    /// <summary>
    /// Returns a string representation of the given <see cref="SchemaInformationType"/> value.
    /// </summary>
    /// <param name="schemaInformation">A <see cref="SchemaInformationType"/> value.</param>
    /// <returns>A string representation of the given <see cref="SchemaInformationType"/> value.</returns>
    public static string ToCollection(this SchemaInformationType schemaInformation)
    {
      switch (schemaInformation)
      {
        case SchemaInformationType.ColumnsFull:
          return "COLUMNS";

        case SchemaInformationType.ForeignKeyColumns:
          return "FOREIGN KEY COLUMNS";

        case SchemaInformationType.ForeignKeys:
          return "FOREIGN KEYS";

        case SchemaInformationType.ProcedureParameters:
          return "PROCEDURE PARAMETERS";

        case SchemaInformationType.ProceduresWithParameters:
          return "PROCEDURES WITH PARAMETERS";

        default:
          return schemaInformation.ToString().ToUpperInvariant();
      }
    }

    /// <summary>
    /// Converts an object array into a dictionaries list.
    /// </summary>
    /// <param name="objectArray">Array of objects.</param>
    /// <returns>A populated dictionaries list if conversion was successful, otherwise the list is empty.</returns>
    public static List<Dictionary<string, object>> ToDictionariesList(object[] objectArray)
    {
      var list = new List<Dictionary<string, object>>();
      if (objectArray == null || objectArray.Length == 0)
      {
        return list;
      }

      list.AddRange(objectArray.Select(objectItem => objectItem as Dictionary<string, object>));
      return list;
    }

    /// <summary>
    /// Transforms the information contained in a boxed BaseShell result to a list of dictionaries of results and information about them.
    /// </summary>
    /// <param name="baseShellResult">A boxed BaseShell result.</param>
    /// <param name="executionTime">Execution time formatted to seconds.</param>
    /// <returns>A list of dictionaries of results and information about them.</returns>
    public static List<Dictionary<string, object>> ToDictionariesList(this object baseShellResult, out string executionTime)
    {
      executionTime = null;
      if (baseShellResult == null)
      {
        return null;
      }

      executionTime = ZERO_EXECUTION_TIME;
      return baseShellResult is BaseResult baseResult
        ? baseResult.ToDictionariesList(out executionTime)
        : UnknownResultToDictionaryList(baseShellResult);
    }

    /// <summary>
    /// Transforms the information contained in a <see cref="BaseResult"/> to a list of dictionaries of results and information about them.
    /// </summary>
    /// <param name="baseResult">A <see cref="BaseResult"/> instance.</param>
    /// <param name="executionTime">Execution time formatted to seconds.</param>
    /// <returns>A list of dictionaries of results and information about them.</returns>
    public static List<Dictionary<string, object>> ToDictionariesList(this BaseResult baseResult, out string executionTime)
    {
      executionTime = null;
      if (baseResult is RowResult rowResult)
      {
        return rowResult.ToDictionariesList(out executionTime);
      }

      if (baseResult is DocResult docResult)
      {
        return docResult.Documents;
      }

      return null;
    }

    public static List<Dictionary<string, object>> ToDictionariesList(this RowResult rowResult, out string executionTime)
    {
      executionTime = null;
      if (rowResult == null)
      {
        return null;
      }

      executionTime = rowResult.ExecutionTime;
      return rowResult.Data.ToList();
    }

    /// <summary>
    /// Creates a new bitmap based on a given bitmap with its colors converted to a grayscale color palette.
    /// </summary>
    /// <param name="original">The bitmap to convert.</param>
    /// <returns>A new bitmap based on a given bitmap with its colors converted to a grayscale color palette.</returns>
    public static Bitmap ToGrayscale(this Bitmap original)
    {
      return original.ChangeColors(StyleableHelper.GrayScaleColorMatrix);
    }

    /// <summary>
    /// Converts an array of bytes into a string in hexadecimal format.
    /// </summary>
    /// <param name="bytes">An array of bytes.</param>
    /// <returns>A string in hexadecimal format representing an array of bytes.</returns>
    public static string ToHexadecimal(this byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
      {
        return null;
      }

      string hex = BitConverter.ToString(bytes);
      return hex.Replace("-", "");
    }

    /// <summary>
    /// Converts a <see cref="MySqlWorkbench.ConnectionsMigrationDelayType"/> to hours.
    /// </summary>
    /// <param name="migrationDelay">A <see cref="MySqlWorkbench.ConnectionsMigrationDelayType"/> value.</param>
    /// <returns>Value in hours.</returns>
    public static int ToHours(this ConnectionsMigrationDelayType migrationDelay)
    {
      switch (migrationDelay)
      {
        case ConnectionsMigrationDelayType.DelayIndefinitely:
          return -1;

        case ConnectionsMigrationDelayType.DelayOneHour:
          return 1;

        case ConnectionsMigrationDelayType.DelayOneDay:
          return 24;

        case ConnectionsMigrationDelayType.DelayOneWeek:
          return 24 * 7;

        case ConnectionsMigrationDelayType.DelayOneMonth:
          return 24 * 30;

        default:
          // ConnectionsMigrationDelayType.None:
          return 0;
      }
    }

    /// <summary>
    /// Creates a new bitmap based on a given bitmap with its colors converted to an inverted color palette.
    /// </summary>
    /// <param name="original">The bitmap to convert.</param>
    /// <returns>A new bitmap based on a given bitmap with its colors converted to an inverted color palette.</returns>
    public static Bitmap ToInverted(this Bitmap original)
    {
      return original.ChangeColors(StyleableHelper.InvertedColorMatrix);
    }

    /// <summary>
    /// Returns the given string with a character in the given position converted to upper or lower case.
    /// </summary>
    /// <param name="text">A text.</param>
    /// <param name="position">The index within the text for the character to change.</param>
    /// <returns>The given string with a character in the given position converted to upper or lower case.</returns>
    public static string ToLowerAt(this string text, int position)
    {
      return ChangeCaseAt(text, position, false);
    }

    /// <summary>
    /// Returns the <see cref="Mode"/> that corresponds to the given <see cref="ScriptLanguageType"/> value.
    /// </summary>
    /// <param name="scriptLanguage">A <see cref="ScriptLanguageType"/> value.</param>
    /// <returns>The <see cref="Mode"/> that corresponds to the given <see cref="ScriptLanguageType"/> value.</returns>
    public static Mode ToMode(this ScriptLanguageType scriptLanguage)
    {
      switch (scriptLanguage)
      {
        case ScriptLanguageType.JavaScript:
          return Mode.JScript;

        case ScriptLanguageType.Python:
          return Mode.Python;

        case ScriptLanguageType.Sql:
          return Mode.SQL;
      }

      return Mode.None;
    }

    /// <summary>
    /// Returns the <see cref="ScriptLanguageType"/> that corresponds to the given <see cref="Mode"/> value.
    /// </summary>
    /// <param name="languageMode">A <see cref="Mode"/> value.</param>
    /// <returns>The <see cref="ScriptLanguageType"/> that corresponds to the given <see cref="Mode"/> value.</returns>
    public static ScriptLanguageType ToScriptLanguageType(this Mode languageMode)
    {
      switch (languageMode)
      {
        case Mode.JScript:
          return ScriptLanguageType.JavaScript;

        case Mode.Python:
          return ScriptLanguageType.Python;

        case Mode.SQL:
          return ScriptLanguageType.Sql;
      }

      return ScriptLanguageType.None;
    }

    /// <summary>
    /// Returns the given string with a character in the given position converted to upper or lower case.
    /// </summary>
    /// <param name="text">A text.</param>
    /// <param name="position">The index within the text for the character to change.</param>
    /// <returns>The given string with a character in the given position converted to upper or lower case.</returns>
    public static string ToUpperAt(this string text, int position)
    {
      return ChangeCaseAt(text, position, false);
    }

    /// <summary>
    /// Converts a given <see cref="UriHostNameType"/> value to a <see cref="ValidHostNameType"/> one.
    /// </summary>
    /// <param name="uriHostNameType">A <see cref="UriHostNameType"/> value.</param>
    /// <returns>A <see cref="ValidHostNameType"/> value.</returns>
    public static ValidHostNameType ToValidHostNameType(this UriHostNameType uriHostNameType)
    {
      switch (uriHostNameType)
      {
        case UriHostNameType.Dns:
          return ValidHostNameType.DNS;

        case UriHostNameType.IPv4:
          return ValidHostNameType.IPv4;

        case UriHostNameType.IPv6:
          return ValidHostNameType.IPv6;

        default:
          return ValidHostNameType.Unknown;
      }
    }

    /// <summary>
    /// Truncates the text given a maximum width and appends an ellipsis at the end of the truncated text.
    /// </summary>
    /// <param name="proposedText">The text to truncate.</param>
    /// <param name="maxWidth">Maximum width to hold the given text.</param>
    /// <param name="font">The Font used for the text.</param>
    /// <returns>A new string with the truncated text.</returns>
    public static string TruncateString(this string proposedText, float maxWidth, Font font)
    {
      string newText = proposedText;
      if (string.IsNullOrEmpty(proposedText))
      {
        return newText;
      }

      const string ELLIPSIS = "...";
      float sizeText = TextRenderer.MeasureText(newText, font).Width;
      if (!(sizeText > maxWidth))
      {
        return newText;
      }

      int index = (int) ((maxWidth/sizeText)*proposedText.Length);
      newText = proposedText.Substring(0, index);
      sizeText = TextRenderer.MeasureText(newText + ELLIPSIS, font).Width;
      if (sizeText < maxWidth)
      {
        while (sizeText < maxWidth)
        {
          newText = proposedText.Substring(0, ++index);
          sizeText = TextRenderer.MeasureText(newText + ELLIPSIS, font).Width;
          if (!(sizeText > maxWidth))
          {
            continue;
          }

          newText = newText.Substring(0, newText.Length - 1);
          break;
        }
      }
      else
      {
        while (sizeText > maxWidth)
        {
          newText = proposedText.Substring(0, --index);
          sizeText = TextRenderer.MeasureText(newText + ELLIPSIS, font).Width;
        }
      }

      newText += ELLIPSIS;
      return newText;
    }

    /// <summary>
    /// Truncates the text given a maximum width and appends an ellipsis at the end of the truncated text.
    /// </summary>
    /// <param name="control">The control where we want to draw the text, normally a label.</param>
    /// <param name="proposedText">The text to truncate.</param>
    /// <param name="maxWidth">Maximum width to hold the given text.</param>
    /// <param name="overridingFont">The Font used for the text, if <c>null</c> the control's Font is used.</param>
    /// <returns>A new string with the truncated text.</returns>
    public static string TruncateString(this Control control, string proposedText, float maxWidth, Font overridingFont = null)
    {
      if (string.IsNullOrEmpty(proposedText))
      {
        return proposedText;
      }

      if (overridingFont == null)
      {
        overridingFont = control.Font;
      }

      return proposedText.TruncateString(maxWidth, overridingFont);
    }

    /// <summary>
    /// Gets an enumeration value whose description matches the given one.
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="description">The description assigned to an enumeration value.</param>
    /// <param name="ignoreCase"><c>true</c> to ignore case; <c>false</c> to consider case.</param>
    /// <param name="result">
    /// Contains an object of type <see cref="TEnum"/> whose value's description matches <seealso cref="description"/>.
    /// If the parse operation fails, result contains the default value of the underlying type of <see cref="TEnum"/>.
    /// Note that this value need not be a member of the <see cref="TEnum"/> enumeration.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>An enumeration value of the given type if it matches the given description.</returns>
    public static bool TryParseFromDescription<TEnum>(this TEnum enumType, string description, bool ignoreCase, out TEnum result) where TEnum : struct
    {
      var theType = typeof(TEnum);
      if (!theType.IsEnum)
      {
        throw new InvalidOperationException(Resources.TEnumNotEnumTypeException);
      }

      var memberInfos = theType.GetMembers();
      result = default(TEnum);
      bool couldParse = false;
      foreach (var memberInfo in memberInfos)
      {
        var attributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length == 0)
        {
          continue;
        }

        var descriptionAttribute = attributes[0] as DescriptionAttribute;
        if (descriptionAttribute == null
            || !descriptionAttribute.Description.Equals(description, StringComparison.Ordinal))
        {
          continue;
        }

        var fieldInfo = memberInfo as FieldInfo;
        if (fieldInfo == null)
        {
          break;
        }

        couldParse = true;
        result = (TEnum)fieldInfo.GetValue(null);
      }

      return couldParse;
    }

    /// <summary>
    /// Attempts to convert an unknown result into a list of dictionaries of results and information about them.
    /// </summary>
    /// <param name="unknownResult">An object that is not a <see cref="BaseResult"/>.</param>
    /// <returns>A list of dictionaries of results and information about them.</returns>
    public static List<Dictionary<string, object>> UnknownResultToDictionaryList(this object unknownResult)
    {
      if (unknownResult is object[])
      {
        return ToDictionariesList((object[]) unknownResult);
      }

      if (unknownResult is string)
      {
        var stringResult = unknownResult.ToString();
        return stringResult.CollectionToDictionaryList(true);
      }

      return null;
    }

    /// <summary>
    /// Fires the <see cref="Control.Validating"/> and <see cref="Control.Validated"/> event methods.
    /// </summary>
    /// <param name="control">A <see cref="Control"/> instance.</param>
    /// <param name="runValidating">Flag indicating whether the "validating" delegate is run.</param>
    /// <param name="runValidated">Flag indicating whether the "validated" delegate is run.</param>
    /// <returns><c>true</c> if the <see cref="Control.Validating"/> did not cancel, <c>false</c> otherwise.</returns>
    public static bool Validate(this Control control, bool runValidating = true, bool runValidated = true)
    {
      if (control == null)
      {
        return true;
      }

      if (runValidating)
      {
        var e = new CancelEventArgs();
        _onValidating.Invoke(control, new object[] {e});
        if (e.Cancel)
        {
          return false;
        }
      }

      if (runValidated)
      {
        _onValidated.Invoke(control, new object[] { EventArgs.Empty });
      }

      return true;
    }

    /// <summary>
    /// Implements word wrapping for the label text.
    /// </summary>
    /// <param name="control">The control where we want to draw the text, normally a label.</param>
    /// <param name="proposedText">The proposed text to be drawn on the control.</param>
    /// <param name="overridingWidth">The width in which the text is to be fit, if the number is <c>0</c> or less the control's Width is used.</param>
    /// <returns>A list of strings containing the proposed text word-wrapped in several lines.</returns>
    public static List<string> WordWrapText(this Control control, string proposedText, int overridingWidth = 0)
    {
      if (control == null)
      {
        return null;
      }

      List<string> wordWrapLines = new List<string>();
      if (control.AutoSize)
      {
        wordWrapLines.Add(proposedText);
        return wordWrapLines;
      }

      if (overridingWidth <= 0)
      {
        overridingWidth = control.Width;
      }

      string remainingText = proposedText.Trim();
      do
      {
        SizeF stringSize = TextRenderer.MeasureText(remainingText, control.Font);
        double trimPercentage = overridingWidth / stringSize.Width;
        string textToDraw;
        if (trimPercentage < 1)
        {
          int lengthToCut = Convert.ToInt32(remainingText.Length * trimPercentage);
          lengthToCut = lengthToCut > 0 ? lengthToCut - 1 : 0;
          int spaceBeforePos = lengthToCut;
          int spaceAfterPos = remainingText.IndexOf(" ", lengthToCut, StringComparison.Ordinal);
          textToDraw = spaceAfterPos >= 0 ? remainingText.Substring(0, spaceAfterPos) : remainingText;
          while (spaceBeforePos > -1 && TextRenderer.MeasureText(textToDraw, control.Font).Width > overridingWidth)
          {
            spaceBeforePos = remainingText.LastIndexOf(" ", spaceBeforePos, StringComparison.Ordinal);
            textToDraw = spaceBeforePos >= 0 ? remainingText.Substring(0, spaceBeforePos) : textToDraw;
            spaceBeforePos--;
          }
        }
        else
        {
          textToDraw = remainingText;
        }

        textToDraw = textToDraw.Trim();
        if (textToDraw.Length > 0)
        {
          wordWrapLines.Add(textToDraw);
        }

        remainingText = textToDraw.Length < remainingText.Length ? remainingText.Substring(textToDraw.Length).Trim() : string.Empty;
      } while (remainingText.Length > 0);

      return wordWrapLines;
    }

    /// <summary>
    /// Writes an 8-byte floating point (<see cref="double"/>) value to the given <see cref="BinaryWriter"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
    /// <param name="value">An 8-byte floating point value (<see cref="double"/>).</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    public static void WriteDoubleUsingByteOrder(this BinaryWriter writer, double value, ByteOrderType byteOrder)
    {
      if (writer == null)
      {
        return;
      }

      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        writer.Write(value);
      }
      else
      {
        writer.Write(value.ReverseBytes());
      }
    }

    /// <summary>
    /// Writes a 4-byte signed integer (<see cref="int"/>) value to the given <see cref="BinaryWriter"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
    /// <param name="value">An 4-byte signed integer (<see cref="int"/>).</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    public static void WriteIntUsingByteOrder(this BinaryWriter writer, int value, ByteOrderType byteOrder)
    {
      if (writer == null)
      {
        return;
      }

      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        writer.Write(value);
      }
      else
      {
        writer.Write(value.ReverseBytes());
      }
    }

    /// <summary>
    /// Writes an 8-byte signed integer (<see cref="long"/>) value to the given <see cref="BinaryWriter"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
    /// <param name="value">An 8-byte signed integer (<see cref="long"/>).</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    public static void WriteLongUsingByteOrder(this BinaryWriter writer, long value, ByteOrderType byteOrder)
    {
      if (writer == null)
      {
        return;
      }

      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        writer.Write(value);
      }
      else
      {
        writer.Write(value.ReverseBytes());
      }
    }

    /// <summary>
    /// Writes a 4-byte unsigned integer (<see cref="uint"/>) value to the given <see cref="BinaryWriter"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
    /// <param name="value">A 4-byte unsigned integer (<see cref="uint"/>).</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    public static void WriteUIntUsingByteOrder(this BinaryWriter writer, uint value, ByteOrderType byteOrder)
    {
      if (writer == null)
      {
        return;
      }

      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        writer.Write(value);
      }
      else
      {
        writer.Write(value.ReverseBytes());
      }
    }

    /// <summary>
    /// Writes an 8-byte unsigned integer (<see cref="ulong"/>) value to the given <see cref="BinaryWriter"/> using the given <see cref="ByteOrderType"/>.
    /// </summary>
    /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
    /// <param name="value">An 8-byte unsigned integer (<see cref="ulong"/>).</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> to interpret the read binary data.</param>
    public static void WriteULongUsingByteOrder(this BinaryWriter writer, ulong value, ByteOrderType byteOrder)
    {
      if (writer == null)
      {
        return;
      }

      var computerArchitectureIsLittleEndian = BitConverter.IsLittleEndian;
      if (computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.NetworkDataRepresentation
          || !computerArchitectureIsLittleEndian && byteOrder == ByteOrderType.ExternalDataRepresentation)
      {
        writer.Write(value);
      }
      else
      {
        writer.Write(value.ReverseBytes());
      }
    }

    /// <summary>
    /// Creates an icon or cursor from an ICONINFO structure.
    /// </summary>
    /// <param name="pIconInfo">A pointer to an ICONINFO structure the function uses to create the icon or cursor.</param>
    /// <returns>If the function succeeds, the return value is a handle to the icon or cursor that is created. Null if fails.</returns>
    [DllImport(DllImportConstants.USER32)]
    private static extern IntPtr CreateIconIndirect(ref IconInfo pIconInfo);

    /// <summary>
    /// Retrieves information about the specified icon or cursor.
    /// </summary>
    /// <param name="hIcon">A handle to the icon or cursor.</param>
    /// <param name="pIconInfo">A pointer to an ICONINFO structure. The function fills in the structure's members.</param>
    /// <returns><c>true</c> if the function succeeds and the function fills in the members of the specified ICONINFO structure, <c>false</c> otherwise.</returns>
    [DllImport(DllImportConstants.USER32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
  }
}