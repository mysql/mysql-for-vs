using System;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Common.CommandTrees;

namespace MySql.Data.Entity
{
    class Metadata
    {
        public static bool IsNumericType(TypeUsage typeUsage)
        {
            PrimitiveType pt = (PrimitiveType)typeUsage.EdmType;

            switch (pt.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Boolean:
                case PrimitiveTypeKind.Byte:
                case PrimitiveTypeKind.Decimal:
                case PrimitiveTypeKind.Double:
                case PrimitiveTypeKind.Single:
                case PrimitiveTypeKind.Int16:
                case PrimitiveTypeKind.Int32:
                case PrimitiveTypeKind.Int64:
                case PrimitiveTypeKind.SByte:
                    return true;
                default:
                    return false;
            }
        }

        public static DbType GetDbType(TypeUsage typeUsage)
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

        public static string GetOperator(DbExpressionKind expressionKind)
        {
            switch (expressionKind)
            {
                case DbExpressionKind.Equals: return "=";
                case DbExpressionKind.LessThan: return "<";
                case DbExpressionKind.GreaterThan: return ">";
                case DbExpressionKind.LessThanOrEquals: return "<=";
                case DbExpressionKind.GreaterThanOrEquals: return ">=";
                case DbExpressionKind.NotEquals: return "!=";
                case DbExpressionKind.LeftOuterJoin: return "LEFT OUTER JOIN";
                case DbExpressionKind.InnerJoin: return "INNER JOIN";
                case DbExpressionKind.CrossJoin: return "CROSS JOIN";
                case DbExpressionKind.FullOuterJoin: return "OUTER JOIN";
            }
            throw new NotSupportedException("expression kind not supported");
        }
    }
}
