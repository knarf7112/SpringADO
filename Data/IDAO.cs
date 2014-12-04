using System.Collections.Generic;

namespace Data
{
    public interface IDAO<TEntity,TPK>
    {
        void Insert(TEntity entity);//到資料庫用該物件插入新資料
        void Update(TEntity entity);//到資料庫用該物件更新資料
        void Delete(TPK pk);//到資料庫用PK刪除該筆資料
        TEntity FindByPK(TPK pk);//到資料庫用pk找單筆資料並轉成資料物件傳回
        IList<TEntity> FindAll();//到資料庫取得所有資料並暫放在列舉
        bool Exist(TPK pk);//到資料庫用pk檢查資料是否存在
    }
}
