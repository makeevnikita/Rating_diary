//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Diary
{
    using System;
    using System.Collections.Generic;
    
    public partial class USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USER()
        {
            this.GROUP = new HashSet<GROUP>();
            this.MARK = new HashSet<MARK>();
            this.SUBJECT = new HashSet<SUBJECT>();
        }
    
        public int USER_ID { get; set; }
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public string PATRONYMIC { get; set; }
        public Nullable<int> FK_GROUP_ID { get; set; }
        public string LOGIN { get; set; }
        public string PASSWORD { get; set; }
        public Nullable<int> FK_ROLE_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUP> GROUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MARK> MARK { get; set; }
        public virtual ROLE ROLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBJECT> SUBJECT { get; set; }
    }
}
