using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IQueryable._01.E3SClient;

namespace IQueryable._01
{
	public class E3SEntitySet<T> : IQueryable<T> where T : E3SEntity
	{
		protected Expression E3SExpression;
		protected IQueryProvider E3SProvider;

		public E3SEntitySet(string user, string password)
		{
			E3SExpression = Expression.Constant(this);

			var client = new E3SQueryClient(user, password);

			E3SProvider = new E3SLinqProvider(client);
		}

		public Type ElementType => typeof(T);

	    public Expression Expression => E3SExpression;

	    public IQueryProvider Provider => E3SProvider;

	    public IEnumerator<T> GetEnumerator()
		{
			return E3SProvider.Execute<IEnumerable<T>>(E3SExpression).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return E3SProvider.Execute<IEnumerable>(E3SExpression).GetEnumerator();
		}
	}
}
