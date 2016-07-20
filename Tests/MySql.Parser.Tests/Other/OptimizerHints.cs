// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Text;
using System.Windows.Forms;
using Xunit;

namespace MySql.Parser.Tests.Other
{
  public class OptimizerHints
  {
    [Fact]
    public void Optimizer_Bka_NoBka()
    {
      string input = "SELECT /*+ BKA(t1) NO_BKA(t2) */ * FROM t1 INNER JOIN t2 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_Bnl_NoBnl()
    {
      string input = "SELECT /*+ BNL(t1) NO_BNL(t2) */ * FROM t1 INNER JOIN t2 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_MaxExecutionTime()
    {
      string input = "SELECT /*+ MAX_EXECUTION_TIME(1000) */ * FROM t1 INNER JOIN t2 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_Mrr_NoMrr()
    {
      string input = "SELECT /*+ MRR(t1) NO_MRR(t2) */ * FROM t1 INNER JOIN t2 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_NoIcp()
    {
      string input = "SELECT /*+ NO_ICP(t1, t2) */ * FROM t1 INNER JOIN t2 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_NoRangeOptimization()
    {
      string input = "SELECT /*+ NO_RANGE_OPTIMIZATION(t3 PRIMARY, f2_idx) */ f1 FROM t3 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_QbName()
    {
      string input = "SELECT /*+ QB_NAME(qb2) */ f1 FROM t3 WHERE f1 > 30 AND f1 < 33; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_SemiJoin_NoSemijoin()
    {
      string input = "SELECT /*+ SEMIJOIN(FIRSTMATCH, LOOSESCAN) NO_SEMIJOIN(@subq1 FIRSTMATCH, LOOSESCAN) */ * FROM t1; ";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }

    [Fact]
    public void Optimizer_Subquery()
    {
      string input = "SELECT /*+ SUBQUERY(MATERIALIZATION) */ a FROM t1;";
      Utility.ParseSql(input, false, new Version(5, 7, 12));
    }
  }
}
