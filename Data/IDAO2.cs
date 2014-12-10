using System.Collections.Generic;

namespace Data
{
    public interface IDAO2<TEntity,TPK,T> : IDAO<TEntity,TPK>
    {
        IList<TEntity> ListCustomerOrders(TPK pk);//增加一個列表方法,列舉出某個客戶(PK)的所有定單(IList(Order))

        /// <summary>
        /// 列舉資料,T表示要搜尋的欄位,TPK表示搜尋欄位的條件值
        /// </summary>
        /// <param name="value">表示要搜尋的數據</param>
        /// <param name="t">表示要搜尋的欄位名稱</param>
        /// <returns></returns>
        IList<TEntity> FindByT(T fieldName, TPK fieldValue);
    }
}
