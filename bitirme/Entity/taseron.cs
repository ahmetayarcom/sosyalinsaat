//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace bitirme.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class taseron
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public taseron()
        {
            this.usta = new HashSet<usta>();
        }
    
        public int ID { get; set; }
        public int hesapID { get; set; }
        public int calisanSayisi { get; set; }
        public Nullable<int> isSayisi { get; set; }
        public Nullable<int> puan { get; set; }
        public string ad { get; set; }
        public string adres { get; set; }
        public Nullable<int> telno { get; set; }
        public string webadresi { get; set; }
        public Nullable<int> issayisi1 { get; set; }
    
        public virtual hesap hesap { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<usta> usta { get; set; }
    }
}