//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace intsis
{
    using System;
    using System.Collections.Generic;
    
    public partial class Answer
    {
        public int ID { get; set; }
        public int IDRule { get; set; }
        public string Ans { get; set; }
        public string NextR { get; set; }
        public string Rec { get; set; }
    
        public virtual Rules Rules { get; set; }
    }
}
