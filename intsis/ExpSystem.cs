
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
    
public partial class ExpSystem
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ExpSystem()
    {

        this.LinearSystem_Question = new HashSet<LinearSystem_Question>();

        this.WeightedSystem_Fact = new HashSet<WeightedSystem_Fact>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public string ScopeOfApplication { get; set; }

    public string Description { get; set; }

    public bool Type { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<LinearSystem_Question> LinearSystem_Question { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<WeightedSystem_Fact> WeightedSystem_Fact { get; set; }

}

}
