using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification(Expression<Func<T,bool>>criteria)
        {
            Criteria= criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; }=new List<Expression<Func<T, object>>>();

        protected void AddInclud(Expression<Func<T, object>> IncludeExpression)
    => Includes.Add(IncludeExpression);



        //Order By
        public Expression<Func<T, object>> OrderByAsc { get; private set; }

        public Expression<Func<T, object>> OrderByDesc { get; private set; }

        protected void AddorderByAsc(Expression<Func<T, object>> orderByAscExpression)
            => OrderByAsc = orderByAscExpression;
        protected void AddorderByDesc(Expression<Func<T, object>> orderByDescExpression)
            => OrderByDesc = orderByDescExpression;


        //Paginated
        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPaginated { get; private set; }

        protected void ApplyPagination(int skip,int take)
        {
            Take= take;
            Skip= skip;
            IsPaginated = true;
        }
    }
}
