using System;
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;

namespace MySql.Data.MySqlClient.Generator
{
    abstract class SqlGenerator : DbExpressionVisitor
    {
        private List<MySqlParameter> parameters;
        protected StringBuilder current;
        protected string tabs = String.Empty;
        private int parameterCount = 1;

        public SqlGenerator()
        {
            parameters = new List<MySqlParameter>();
        }

        #region Properties

        public List<MySqlParameter> Parameters
        {
            get { return parameters; }
        }

        #endregion

        protected void Push()
        {
            tabs += "-";
        }

        protected void Pop()
        {
            tabs = tabs.Substring(1);
        }

        public abstract string GenerateSQL(DbCommandTree commandTree);

        protected string QuoteIdentifier(string id)
        {
            return String.Format("`{0}`", id);
        }

        protected DbType GetDbType(TypeUsage typeUsage)
        {
            PrimitiveType pt = (PrimitiveType)typeUsage.EdmType;
            
            switch (pt.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary: return DbType.Binary;
                case PrimitiveTypeKind.Boolean: return DbType.Boolean;
                case PrimitiveTypeKind.Byte: return DbType.Byte;
                case PrimitiveTypeKind.DateTime: return DbType.DateTime;
                case PrimitiveTypeKind.Decimal: return DbType.Decimal;
                case PrimitiveTypeKind.Double: return DbType.Double;
                case PrimitiveTypeKind.Single: return DbType.Single;
                case PrimitiveTypeKind.Guid: return DbType.Guid;
                case PrimitiveTypeKind.Int16: return DbType.Int16;
                case PrimitiveTypeKind.Int32: return DbType.Int32;
                case PrimitiveTypeKind.Int64: return DbType.Int64;
                case PrimitiveTypeKind.SByte: return DbType.SByte;
                case PrimitiveTypeKind.String: return DbType.String;
//                case PrimitiveTypeKind.UInt16: return DbType.UInt16;
//                case PrimitiveTypeKind.UInt32: return DbType.UInt32;
//                case PrimitiveTypeKind.UInt64: return DbType.UInt64;
                default:
                    throw new InvalidOperationException(
                        string.Format("Unknown PrimitiveTypeKind {0}", pt.PrimitiveTypeKind));
            }            
        }

        protected string CreateUniqueParameterName()
        {
            return String.Format("@p{0}", parameterCount++);
        }

        #region DbExpressionVisitor Abstract methods

//        public override void Visit(DbViewExpression expression)
//        {
//            throw new System.NotImplementedException();
//        }

        public override void Visit(DbVariableReferenceExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbUnionAllExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbTreatExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbSortExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbSkipExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbScanExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "ScanExpression", expression.Target.Name));
            EntitySetBase target = expression.Target;
            current.AppendFormat("`{0}`.`{1}`", target.EntityContainer, target.Name);
        }

        public override void Visit(DbRelationshipNavigationExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbRefExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbQuantifierExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbPropertyExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbProjectExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbParameterReferenceExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbOrExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbOfTypeExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbNullExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbNotExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbNewInstanceExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbLimitExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbLikeExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbJoinExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbIsOfExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbIsNullExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbIsEmptyExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbIntersectExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbGroupByExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbRefKeyExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbEntityRefExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbFunctionExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbFilterExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbExceptExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbElementExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbDistinctExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbDerefExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbCrossJoinExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbConstantExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbComparisonExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbCastExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbCaseExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbArithmeticExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbApplyExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbAndExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public override void Visit(DbExpression expression)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
