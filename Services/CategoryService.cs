using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace WPFAShopMgt23.Services
{
    public class CategoryService
    {
        private readonly ShopDbContext _db;
        public CategoryService(ShopDbContext context) {
            _db = context;
        }
        public List<Category> GetAllCategories() => _db.Categories.ToList();
        public  int GetCategoryIdByName(string categoryName)
        {
            return _db.Categories.Where(cate => cate.Name == categoryName).Select(c => c.Id).FirstOrDefault();
        }
        public int GetCategoryIdFromView(Category selected)
        {
            //Category Selected = ProductCateComboBox.SelectedItem as Category;
            return selected.Id;
        }
        public Category GetCategorybyId(int id) => _db.Categories.First(cate => cate.Id == id);

        public int CateIdToComboBoxIndexConverter(Product product)
        {
            var CateList = GetAllCategories();
            Category cate = GetCategorybyId((int)product.CatId);
            return CateList.IndexOf(cate);
        }
    }
}
