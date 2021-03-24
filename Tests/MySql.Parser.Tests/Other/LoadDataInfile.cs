// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Text;
using Antlr.Runtime;
using Xunit;


namespace MySql.Parser.Tests
{
  
  public class LoadDataInfile
  {
    [Fact]
    public void Simple()
    {
      Utility.ParseSql("LOAD DATA INFILE 'data.txt' INTO TABLE db2.my_table;");
    }

    [Fact]
    public void Simple2()
    {
      Utility.ParseSql(@"
LOAD DATA INFILE 'data.txt' INTO TABLE db2.my_table
FIELDS TERMINATED BY '\t' ENCLOSED BY '' ESCAPED BY '\\'
LINES TERMINATED BY '\n' STARTING BY '';");
    }

    [Fact]
    public void Simple3()
    {
      Utility.ParseSql(@"LOAD DATA INFILE '/tmp/test.txt' INTO TABLE test
  FIELDS TERMINATED BY ','  LINES STARTING BY 'xxx';");
    }

    [Fact]
    public void Simple4()
    {
      Utility.ParseSql(@"SELECT * INTO OUTFILE 'data.txt'
  FIELDS TERMINATED BY ','
  FROM table2;");
    }

    [Fact]
    public void Simple5()
    {
      Utility.ParseSql(@"LOAD DATA INFILE 'data.txt' INTO TABLE table2
  FIELDS TERMINATED BY ',';");
    }

    [Fact]
    public void Simple6()
    {
      Utility.ParseSql(@"LOAD DATA INFILE 'data.txt' INTO TABLE table2
  FIELDS TERMINATED BY '\t';");
    }

    [Fact]
    public void Simple7()
    {
      Utility.ParseSql(
        "LOAD DATA INFILE 'data.txt' INTO TABLE tbl_name  FIELDS TERMINATED BY ',' ENCLOSED BY '\"'  LINES TERMINATED BY '\\r\\n'  IGNORE 1 LINES;");
    }

    [Fact]
    public void Simple8()
    {
      Utility.ParseSql("LOAD DATA INFILE 'persondata.txt' INTO TABLE persondata;");
    }
    
    [Fact]
    public void Simple9()
    {
      Utility.ParseSql("LOAD DATA INFILE 'persondata.txt' INTO TABLE persondata (col1,col2);");
    }

    [Fact]
    public void Simple10()
    {
      Utility.ParseSql(@"LOAD DATA INFILE 'file.txt'
  INTO TABLE t1
  (column1, @var1)
  SET column2 = @var1/100;
");
    }

    [Fact]
    public void Simple11()
    {
      Utility.ParseSql(@"LOAD DATA INFILE 'file.txt'
  INTO TABLE t1
  (column1, column2)
  SET column3 = CURRENT_TIMESTAMP;");
    }

    [Fact]
    public void Simple12()
    {
      Utility.ParseSql(@"
LOAD DATA INFILE 'file.txt'
  INTO TABLE t1
  (column1, @dummy, column2, @dummy, column3);");
    }

    [Fact]
    public void Simple13()
    {
      Utility.ParseSql(@"
LOAD DATA INFILE '/tmp/bit_test.txt'
  INTO TABLE bit_test (@var1) SET b= CAST(@var1 AS UNSIGNED);");
    }

//    [Fact]
//    public void Simple14()
//    {
//      Utility.ParseSql(@"LOAD DATA INFILE 'C:/bobsfile.txt' INTO TABLE mydatabase.mytable 
//FIELDS TERMINATED BY '' 
//FIELDS ENCLOSED BY ''; ");
//    }

    [Fact]
    public void Simple15()
    {
      Utility.ParseSql(@"load data infile '/tmp/xxx.dat' 
into table xxx 
fields terminated by '|' 
lines terminated by '\n' 
(col1, 
col2, 
@col3, 
@col4, 
col5) 
set 
col3 = str_to_date(@col3, '%m/%d/%Y'), 
col4 = str_to_date(@col4, '%d/%m/%Y');");
    }

    [Fact]
    public void Simple16()
    {
      Utility.ParseSql(@"LOAD DATA LOCAL INFILE '<file name>' INTO TABLE WindData
(@var1)
SET Date=str_to_date(SUBSTR(@var1,3,10),'%m/%d/%Y'), 
Time=SUBSTR(@var1,14,8), 
WindVelocity=SUBSTR(@var1,26,5),
WindDirection=SUBSTR(@var1,33,3),
WindCompass=SUBSTR(@var1,38,3),
WindNorth=SUBSTR(@var1,43,6),
WindEast=SUBSTR(@var1,51,6),
WindSamples=SUBSTR(@var1,61,4);");
    }

    [Fact]
    public void Simple17()
    {
      Utility.ParseSql(@"LOAD DATA LOCAL INFILE 'C:/path/to/mytable.txt' IGNORE
INTO TABLE mytable
FIELDS TERMINATED BY '\t' LINES TERMINATED BY '\r\n'
(int_col, @float_col)
SET float_col = replace(@float_col, ',', '.');
");
    }

    //[Fact]
    //public void Simple18()
    //{
    //  Utility.ParseSql("LOAD DATA INFILE '/data/input/myinfile.txt';");
    //}

    [Fact]
    public void Simple19()
    {
      Utility.ParseSql(@"LOAD DATA INFILE '/tmp/names.dmp' IGNORE INTO TABLE Names (@var)
SET
Name=Trim(SUBSTR(@var,2,17)),
Gender=SUBSTR(@var,1,1);");
    }

    [Fact]
    public void Simple20()
    {
      Utility.ParseSql(
        "LOAD DATA INFILE '#{DATA_FILE_NAME}' IGNORE INTO TABLE zipcodes FIELDS TERMINATED BY ',' ENCLOSED BY '\"' " +
        "LINES TERMINATED BY '\\n' (city, state_code, @zip, area_code, county_fips, county_name, @preferred, timezone, dst, lat, " +
        "lon, msa, pmsa, @city_abbreviation, ma, zip_type) SET allow_registrations = 1, zip = IF(@preferred='P', @zip, NULL);");
    }

    [Fact]
    public void WithPartition_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"LOAD DATA INFILE '#{DATA_FILE_NAME}' IGNORE
INTO TABLE zipcodes partition ( p1 )
FIELDS TERMINATED BY ','
ENCLOSED BY '""'
LINES TERMINATED BY '\n'
(city, state_code, @zip, area_code, county_fips, county_name, @preferred, timezone, dst, lat,
lon, msa, pmsa, @city_abbreviation, ma, zip_type)
SET allow_registrations = 1, zip = IF(@preferred='P', @zip, NULL)	 
;", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("partition") != -1);
    }

    [Fact]
    public void WithPartition_56()
    {
      Utility.ParseSql(
        @"LOAD DATA INFILE '#{DATA_FILE_NAME}' IGNORE
INTO TABLE zipcodes partition ( p1 )
FIELDS TERMINATED BY ','
ENCLOSED BY '""'
LINES TERMINATED BY '\n'
(city, state_code, @zip, area_code, county_fips, county_name, @preferred, timezone, dst, lat,
lon, msa, pmsa, @city_abbreviation, ma, zip_type)
SET allow_registrations = 1, zip = IF(@preferred='P', @zip, NULL)	 
;", false, new Version(5, 6));
    }
  }
}
