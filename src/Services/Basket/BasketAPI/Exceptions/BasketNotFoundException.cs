﻿namespace BasketAPI.Exceptions
{
    public class BasketNotFoundException : NotFountException
    {
        public BasketNotFoundException(string userName) : base("Basket", userName)
        {
        }
    }
}
