﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Qactive.Expressions
{
  [Serializable]
  internal sealed class SerializableInvocationExpression : SerializableExpression
  {
    public readonly IList<SerializableExpression> Arguments;
    public readonly SerializableExpression Expr;

    public SerializableInvocationExpression(InvocationExpression expression, SerializableExpressionConverter converter)
      : base(expression)
    {
      Arguments = converter.Convert(expression.Arguments);
      Expr = converter.Convert(expression.Expression);
    }

    internal override Expression Convert() => Expression.Invoke(
                                                Expr.TryConvert(),
                                                Arguments.TryConvert());
  }
}