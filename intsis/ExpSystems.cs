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
    
    public partial class ExpSystems
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExpSystems()
        {
            this.Facts = new HashSet<Facts>();
            this.Questions = new HashSet<Questions>();
        }
    
        public int ExpSysID { get; set; }
        public string NameSys { get; set; }
        public string Description { get; set; }
        public string ScopeOfApplication { get; set; }
        public int TypeID { get; set; }
    
        public virtual TypeOfSys TypeOfSys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Facts> Facts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Questions> Questions { get; set; }
    }
}
