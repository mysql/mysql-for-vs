﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("ModelFirstModel1", "StudentKardex", "Student", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(MySql.Data.Entity.Tests.Student), "Kardex", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(MySql.Data.Entity.Tests.Kardex), true)]

#endregion

namespace MySql.Data.Entity.Tests
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class ModelFirstModel1Container : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new ModelFirstModel1Container object using the connection string found in the 'ModelFirstModel1Container' section of the application configuration file.
        /// </summary>
        public ModelFirstModel1Container() : base("name=ModelFirstModel1Container", "ModelFirstModel1Container")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new ModelFirstModel1Container object.
        /// </summary>
        public ModelFirstModel1Container(string connectionString) : base(connectionString, "ModelFirstModel1Container")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new ModelFirstModel1Container object.
        /// </summary>
        public ModelFirstModel1Container(EntityConnection connection) : base(connection, "ModelFirstModel1Container")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Student> Students
        {
            get
            {
                if ((_Students == null))
                {
                    _Students = base.CreateObjectSet<Student>("Students");
                }
                return _Students;
            }
        }
        private ObjectSet<Student> _Students;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Kardex> Kardexes
        {
            get
            {
                if ((_Kardexes == null))
                {
                    _Kardexes = base.CreateObjectSet<Kardex>("Kardexes");
                }
                return _Kardexes;
            }
        }
        private ObjectSet<Kardex> _Kardexes;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Students EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToStudents(Student student)
        {
            base.AddObject("Students", student);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Kardexes EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToKardexes(Kardex kardex)
        {
            base.AddObject("Kardexes", kardex);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="ModelFirstModel1", Name="Kardex")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Kardex : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Kardex object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        /// <param name="studentId">Initial value of the StudentId property.</param>
        /// <param name="score">Initial value of the Score property.</param>
        public static Kardex CreateKardex(global::System.Int32 id, global::System.Int32 studentId, global::System.Double score)
        {
            Kardex kardex = new Kardex();
            kardex.Id = id;
            kardex.StudentId = studentId;
            kardex.Score = score;
            return kardex;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 StudentId
        {
            get
            {
                return _StudentId;
            }
            set
            {
                OnStudentIdChanging(value);
                ReportPropertyChanging("StudentId");
                _StudentId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("StudentId");
                OnStudentIdChanged();
            }
        }
        private global::System.Int32 _StudentId;
        partial void OnStudentIdChanging(global::System.Int32 value);
        partial void OnStudentIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Double Score
        {
            get
            {
                return _Score;
            }
            set
            {
                OnScoreChanging(value);
                ReportPropertyChanging("Score");
                _Score = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Score");
                OnScoreChanged();
            }
        }
        private global::System.Double _Score;
        partial void OnScoreChanging(global::System.Double value);
        partial void OnScoreChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("ModelFirstModel1", "StudentKardex", "Student")]
        public Student Student
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Student>("ModelFirstModel1.StudentKardex", "Student").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Student>("ModelFirstModel1.StudentKardex", "Student").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Student> StudentReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Student>("ModelFirstModel1.StudentKardex", "Student");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Student>("ModelFirstModel1.StudentKardex", "Student", value);
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="ModelFirstModel1", Name="Student")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Student : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Student object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        public static Student CreateStudent(global::System.Int32 id, global::System.String name)
        {
            Student student = new Student();
            student.Id = id;
            student.Name = name;
            return student;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("ModelFirstModel1", "StudentKardex", "Kardex")]
        public EntityCollection<Kardex> Kardexes
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Kardex>("ModelFirstModel1.StudentKardex", "Kardex");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Kardex>("ModelFirstModel1.StudentKardex", "Kardex", value);
                }
            }
        }

        #endregion
    }

    #endregion
    
}
