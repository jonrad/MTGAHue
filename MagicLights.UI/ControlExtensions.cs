using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace MagicLights.UI
{
    public static class ControlExtensions
    {
        public static void Bind<TControl, TValue>(
            this TControl control,
            Expression<Func<TControl, TValue>> controlExpression,
            Expression<Func<TValue>> modelExpression)
            where TControl : Control
        {
            var propertyName = GetPropertyName(controlExpression);
            var model = GetModel(modelExpression);
            var modelDataMember = GetModelProperty(modelExpression);
            control.DataBindings.Add(
                propertyName,
                model,
                modelDataMember,
                false,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private static string GetModelProperty<TValue>(Expression<Func<TValue>> modelExpression)
        {
            return ((MemberExpression)modelExpression.Body).Member.Name;
        }

        private static object GetModel<TValue>(Expression<Func<TValue>> modelExpression)
        {
            var child = ((MemberExpression)modelExpression.Body).Expression;
            return Expression.Lambda<Func<object>>(
                child,
                new ParameterExpression[] { })
                .Compile()
                .Invoke();
        }

        private static string GetPropertyName<TControl, TValue>(Expression<Func<TControl, TValue>> controlExpression)
            where TControl : Control
        {
            var memberExpression = (MemberExpression)controlExpression.Body;
            return ((PropertyInfo)memberExpression.Member).Name;
        }
    }
}
