﻿using System;
using System.Linq.Expressions;

namespace Qactive.Expressions
{
  [Serializable]
  internal sealed class SerializableTypeBinaryExpression : SerializableExpression
  {
    public readonly SerializableExpression Expr;
    public readonly Type TypeOperand;

    public SerializableTypeBinaryExpression(TypeBinaryExpression expression, SerializableExpressionConverter converter)
      : base(expression)
    {
      Expr = converter.Convert(expression.Expression);
      TypeOperand = expression.TypeOperand;
    }

    internal override Expression Convert() => Expression.TypeIs(
                                                Expr.TryConvert(),
                                                TypeOperand);
  }
}