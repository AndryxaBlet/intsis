
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
    
public partial class WeightFactAnswer
{

    public int Id { get; set; }

    public int IdAnswer { get; set; }

    public int IdFact { get; set; }

    public bool PlusOrMinus { get; set; }

    public decimal Weight { get; set; }



    public virtual WeightedSystem_Answer WeightedSystem_Answer { get; set; }

}

}
