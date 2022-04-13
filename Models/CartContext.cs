using Newtonsoft.Json;

namespace web_bh.Models
{
    public class CartContext
    {
        public string path = @"wwwroot/cart.json";
        public Cart cart = new Cart()
        {
            carts = new List<CartData>()
        };
        public CartContext()
        {
            ReadData();
        }
        public void Add(CartData data)
        {
            bool isUser = false;
            foreach (var i in cart.carts.ToList())
            {
                if(i.id_user == data.id_user)
                {
                    isUser = true;
                    foreach (var p in data.cartItems.ToList())
                    {
                        bool isProduct = true;
                        foreach (var q in i.cartItems.ToList())
                        {
                            if(p.id_product == q.id_product)
                            {
                                q.quantity += p.quantity;
                                isProduct = false;
                                break;
                            }
                        }
                        if(isProduct)
                        {
                            i.cartItems.Add(p);
                        }
                    }
                }
            }
            if(!isUser)
            {
               cart.carts.Add(data);
            }
        }
        public void Remove(CartData data)
        {
            bool isUser = false;
            foreach (var i in cart.carts)
            {
                if(i.id_user == data.id_user)
                {
                    isUser = true;
                    bool isProduct = false;
                    foreach (var p in i.cartItems.ToList())
                    {
                        foreach (var q in data.cartItems.ToList())
                        {
                            if(p.id_product == q.id_product)
                            {
                                p.quantity -= q.quantity;
                            }
                            if(p.quantity <= 0)
                            {
                                i.cartItems.Remove(p);
                            }
                        }
                    }
                    if(!isProduct)
                    {
                        foreach (var j in data.cartItems.ToList())
                        {
                            i.cartItems.Remove(j);
                        }
                    }
                }
            }
            if(!isUser)
            {
               cart.carts.Remove(data);
            }
        }
        public int getQuantity(int idUser, int idProduct)
        {
            var cartData = getCartData(idUser);
            if(cartData.cartItems == null)
            {
                return 0;
            }else
            {
                var CartItem = cartData.cartItems.Where(
                    p => p.id_product.Equals(idProduct) 
                ).FirstOrDefault();
                if(CartItem == null)
                {
                    return 0;
                }else
                {
                    return CartItem.quantity;
                }
            }
        }
        public CartData getCartData(int id)
        {
            CartData a = new CartData()
            {
                id_user = id,
                cartItems = new List<CartItem>()
            };
            if(cart == null)
            {
                return a;
            }
            foreach (var item in cart.carts.ToList())
            {
                if(item.id_user == id)
                {
                    a = item;
                }
            }
            return a;
            
        }
        public void ReadData()
        {
            using(StreamReader sr = File.OpenText(path))
            {
                var obj = sr.ReadToEnd();
                cart = JsonConvert.DeserializeObject<Cart>(obj);
            }
        }
        public bool WriteData()
        {
            try
            {
                if (cart != null)
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        var cartData = JsonConvert.SerializeObject(cart);
                        sw.WriteLine(cartData);
                    }
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
