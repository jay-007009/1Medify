using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using OneMedify.Infrastructure.Entities;

namespace OneMedify.Infrastructure.Data
{
    public class OneMedifyDbContext : DbContext
    {
        public OneMedifyDbContext()
        {
        }

        public OneMedifyDbContext(DbContextOptions<OneMedifyDbContext> options) : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<DoctorActionLog> DoctorActionLogs { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientDisease> PatientDiseases { get; set; }
        public virtual DbSet<Pharmacy> Pharmacies { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
        public virtual DbSet<PrescriptionUpload> PrescriptionUploads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var smallintToIntConverter = new ValueConverter<int, short>(
                            intValue => (short)intValue,
                            shortValue => shortValue,


                    new ConverterMappingHints(valueGeneratorFactory: (property, type) => new TemporaryIntValueGenerator()));


            var tinyintToIntConverter = new ValueConverter<int, byte>(
                            intValue => (byte)intValue,
                            byteValue => byteValue,


                    new ConverterMappingHints(valueGeneratorFactory: (property, type) => new TemporaryIntValueGenerator()));


            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City", "Reference");

                entity.Property(city => city.CityId)
                    .HasColumnName("CityID").HasColumnType("smallint").HasConversion(smallintToIntConverter)
                    .ValueGeneratedNever();

                entity.Property(city => city.CityName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(city => city.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(city => city.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(city => city.ModifiedDate).HasColumnType("datetime2(0)");
            });

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.ToTable("Medicine", "Prescription");

                entity.Property(e => e.MedicineId).HasConversion(tinyintToIntConverter).HasColumnName("MedicineID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.DiseaseId).HasConversion(tinyintToIntConverter).HasColumnName("DiseaseID");

                entity.Property(e => e.MedicineName).IsRequired().HasMaxLength(100).IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.CreatedBy).HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.Medicines)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Medicine_Disease");
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("Prescription", "Prescription");

                entity.Property(e => e.PrescriptionId).HasConversion(smallintToIntConverter).HasColumnName("PrescriptionID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.DoctorId).HasConversion(smallintToIntConverter).HasColumnName("DoctorID");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.PatientId).HasConversion(smallintToIntConverter).HasColumnName("PatientID");

                entity.Property(e => e.PrescriptionStatus).HasConversion(tinyintToIntConverter).HasColumnName("PrescriptionStatus");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescription_Doctor");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescription_Patient");
            });

            modelBuilder.Entity<PrescriptionMedicine>(entity =>
            {
                entity.ToTable("PrescriptionMedicine", "Prescription");

                entity.Property(e => e.PrescriptionMedicineId).HasColumnName("PrescriptionMedicineID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.MedicineId).HasConversion(tinyintToIntConverter).HasColumnName("MedicineID");

                entity.Property(e => e.MedicineTiming).IsRequired().HasMaxLength(15).IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.PrescriptionId).HasConversion(smallintToIntConverter).HasColumnName("PrescriptionID");

                entity.Property(e => e.MedicineDosage).HasConversion(tinyintToIntConverter).HasColumnName("MedicineDosage");

                entity.Property(e => e.MedicineDays).HasConversion(tinyintToIntConverter).HasColumnName("MedicineDays");

                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);


                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.PrescriptionMedicines)
                    .HasForeignKey(d => d.MedicineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrescriptionMedicine_Medicine");

                entity.HasOne(d => d.Prescription)
                    .WithMany(p => p.PrescriptionMedicines)
                    .HasForeignKey(d => d.PrescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrescriptionMedicine_Prescription");
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.ToTable("Disease", "Patient");

                entity.Property(disease => disease.DiseaseId).HasColumnName("DiseaseID").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);
                entity.Property(disease => disease.CreatedBy).HasColumnName("CreatedBy").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);
                entity.Property(disease => disease.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(disease => disease.CreatedBy).HasColumnName("CreatedBy").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(disease => disease.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(disease => disease.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(disease => disease.DiseaseName)
                    .IsRequired()
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(disease => disease.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(disease => disease.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(disease => disease.CreatedBy).HasColumnType("tinyint").HasConversion(tinyintToIntConverter);
                entity.Property(disease => disease.ModifiedBy).HasColumnType("tinyint").HasConversion(tinyintToIntConverter);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor", "Doctor");

                entity.HasIndex(doctor => doctor.Email)
                    .HasName("UK_Doctor_Email")
                    .IsUnique();

                entity.HasIndex(doctor => doctor.MobileNumber)
                    .HasName("UK_Doctor_MobileNumber")
                    .IsUnique();

                entity.Property(doctor => doctor.DoctorId).HasColumnName("DoctorID").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(doctor => doctor.Address)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.CityId).HasColumnName("CityID").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(doctor => doctor.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(doctor => doctor.DoctorDegreeFileName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.DoctorDegreeFilePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(doctor => doctor.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.Gender)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.InstituteCertificateFileName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.InstituteCertificateFilePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(doctor => doctor.InstituteEstablishmentDate).HasColumnType("date");

                entity.Property(doctor => doctor.InstituteName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(doctor => doctor.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(doctor => doctor.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(doctor => doctor.ProfilePictureFileName)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(doctor => doctor.ProfilePictureFilePath).IsUnicode(false);

                entity.Property(doctor => doctor.CreatedBy).HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(doctor => doctor.Specialization)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(city => city.City)
                   .WithMany(doctor => doctor.Doctor)
                   .HasForeignKey(doctor => doctor.CityId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Doctor_City");
            });

            modelBuilder.Entity<DoctorActionLog>(entity =>
            {
                entity.ToTable("DoctorActionLog", "Doctor");
                entity.HasOne(d => d.Doctor).WithMany(da => da.DoctorActionLogs).HasForeignKey(r => r.DoctorId);
                entity.HasOne(p => p.Prescription).WithMany(da => da.DoctorActionLogs).HasForeignKey(r => r.PrescriptionId);


                entity.Property(e => e.DoctorActionLogId)
                     .HasColumnName("DoctorActionLogID");

                entity.Property(e => e.DoctorId)
                    .HasColumnName("DoctorID")
                    .HasColumnType("smallint")
                    .HasConversion(smallintToIntConverter);

                entity.Property(e => e.PrescriptionId)
                    .HasColumnName("PrescriptionID")
                    .HasColumnType("smallint")
                    .HasConversion(smallintToIntConverter);

                entity.Property(e => e.PrescriptionStatus)
                    .HasColumnName("PrescriptionStatus")
                    .HasColumnType("tinyint")
                    .HasConversion(tinyintToIntConverter);

                entity.Property(e => e.ActionTimeStamp)
                .HasColumnType("datetime2(0)");

                entity.Property(e => e.DoctorList)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient", "Patient");

                entity.HasIndex(patient => patient.Email)
                    .HasName("UK_Patient_Email")
                    .IsUnique();

                entity.HasIndex(patient => patient.MobileNumber)
                    .HasName("UK_Patient_MobileNumber")
                    .IsUnique();

                entity.Property(patient => patient.PatientId).HasColumnName("PatientID").HasColumnType("smallint").HasConversion(smallintToIntConverter);


                entity.Property(patient => patient.Address)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(patient => patient.CityId).HasColumnName("CityID").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(patient => patient.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(patient => patient.DateOfBirth).HasColumnType("date");

                entity.Property(patient => patient.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(patient => patient.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(patient => patient.Gender)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(patient => patient.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(patient => patient.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(patient => patient.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(patient => patient.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(patient => patient.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(patient => patient.ProfilePictureFileName)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(patient => patient.ProfilePictureFilePath).IsUnicode(false);

                entity.Property(patient => patient.CreatedBy).HasColumnName("CreatedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.HasOne(city => city.City)
                    .WithMany(patient => patient.Patient)
                    .HasForeignKey(patient => patient.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Patient_City");

                entity.Property(patient => patient.ModifiedBy).HasColumnType("smallint").HasConversion(smallintToIntConverter);
            });

            modelBuilder.Entity<PatientDisease>(entity =>
            {
                entity.ToTable("PatientDisease", "Patient");

                entity.Property(patientdisease => patientdisease.PatientDiseaseId)
                .HasColumnName("PatientDiseaseID")
                .HasColumnType("smallint")
                .HasConversion(smallintToIntConverter);

                entity.Property(patientdisease => patientdisease.CreatedBy)
                .HasColumnName("CreatedBy")
                .HasColumnType("smallint")
                .HasConversion(smallintToIntConverter);

                entity.Property(patientdisease => patientdisease.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(patientdisease => patientdisease.DiseaseId)
                .HasColumnName("DiseaseID")
                .HasColumnType("tinyint")
                .HasConversion(tinyintToIntConverter);

                entity.Property(patientdisease => patientdisease.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(patientdisease => patientdisease.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(patientdisease => patientdisease.PatientId)
                .HasColumnName("PatientID")
                .HasColumnType("smallint")
                .HasConversion(smallintToIntConverter);

                entity.Property(patientdisease => patientdisease.ModifiedBy)
                .HasColumnName("ModifiedBy")
                .HasColumnType("smallint")
                .HasConversion(smallintToIntConverter);

                entity.HasOne(disease => disease.Disease)
                    .WithMany(disease => disease.PatientDisease)
                    .HasForeignKey(patientdisease => patientdisease.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientDisease_Disease");

                entity.HasOne(patient => patient.Patient)
                    .WithMany(patient => patient.PatientDisease)
                    .HasForeignKey(patientdisease => patientdisease.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientDisease_Patient");
            });

            modelBuilder.Entity<Pharmacy>(entity =>
            {
                entity.ToTable("Pharmacy", "Pharmacy");

                entity.HasIndex(pharmacy => pharmacy.Email)
                    .HasName("UK_Pharmacy_Email")
                    .IsUnique();

                entity.HasIndex(pharmacy => pharmacy.MobileNumber)
                    .HasName("UK_Pharmacy_MobileNumber")
                    .IsUnique();

                entity.Property(pharmacy => pharmacy.PharmacyId).HasColumnName("PharmacyID").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(pharmacy => pharmacy.Address)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.CityId).HasColumnName("CityID").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(pharmacy => pharmacy.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(pharmacy => pharmacy.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(pharmacy => pharmacy.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.ModifiedDate).HasColumnType("datetime2(0)");
                entity.Property(pharmacy => pharmacy.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(pharmacy => pharmacy.PharmacistDegreeFileName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.PharmacistDegreeFilePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.PharmacyCertificateFileName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.PharmacyCertificateFilePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.PharmacyEstablishmentDate).HasColumnType("date");

                entity.Property(pharmacy => pharmacy.PharmacyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.ProfilePictureFileName)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(pharmacy => pharmacy.ProfilePictureFilePath).IsUnicode(false);

                entity.Property(pharmacy => pharmacy.CreatedBy).HasColumnType("smallint").HasConversion(smallintToIntConverter);
                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State", "Reference");

                entity.Property(state => state.StateId).HasColumnName("StateID").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(state => state.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(state => state.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(state => state.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(state => state.StateName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.ToTable("Medicine", "Prescription");

                entity.HasIndex(e => e.IsDisable)
                    .HasName("IDX_Medicine_IsDisable");

                entity.HasIndex(e => new { e.MedicineId, e.MedicineName, e.DiseaseId })
                    .HasName("IDX_Medicine_DiseaseID");

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(e => e.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.MedicineName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.Medicines)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Medicine_Disease");
            });

            modelBuilder.Entity<PrescriptionMedicine>(entity =>
            {
                entity.ToTable("PrescriptionMedicine", "Prescription");

                entity.HasIndex(e => e.IsDisable)
                    .HasName("IDX_PrescriptionMedicine_IsDisable");

                entity.HasIndex(e => new { e.MedicineId, e.PrescriptionId })
                    .HasName("IDX_PrescriptionMedicine_PrescriptionID");

                entity.Property(e => e.PrescriptionMedicineId).HasColumnName("PrescriptionMedicineID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.MedicineId).HasColumnName("MedicineID").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(e => e.MedicineTiming)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.MedicineDosage).HasColumnName("MedicineDosage").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(e => e.MedicineDays).HasColumnName("MedicineDays").HasColumnType("tinyint").HasConversion(tinyintToIntConverter);

                entity.Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.PrescriptionId).HasColumnType("smallint").HasColumnName("PrescriptionID").HasConversion(smallintToIntConverter);

                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.PrescriptionMedicines)
                    .HasForeignKey(d => d.MedicineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrescriptionMedicine_Medicine");

                entity.HasOne(d => d.Prescription)
                    .WithMany(p => p.PrescriptionMedicines)
                    .HasForeignKey(d => d.PrescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrescriptionMedicine_Prescription");
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.Property(prescription => prescription.PrescriptionId).HasColumnName("PrescriptionID").HasColumnType("smallint").HasConversion(smallintToIntConverter);

                entity.HasOne(prescription => prescription.Pharmacy).WithMany(pharmacy => pharmacy.Prescriptions);

                entity.HasOne(prescription => prescription.SentFromPharmacy).WithMany(pharmacy => pharmacy.SentForApprovalPrescriptions).HasForeignKey(k => k.ModifiedByPharmacy);

                entity.HasOne(prescription => prescription.Doctor).WithMany(doctor => doctor.Prescriptions);

                entity.HasOne(prescription => prescription.ApprovedByDoctor).WithMany(doctor => doctor.ModifiedPrescriptions).HasForeignKey(k => k.ModifiedByDoctor);

                entity.HasOne(prescription => prescription.Patient).WithMany(patient => patient.Prescriptions);

                entity.HasOne(prescription => prescription.SentFromPatient).WithMany(patient => patient.SentForApprovalPrescriptions).HasForeignKey(k => k.ModifiedByPatient);

                entity.HasMany(prescription => prescription.PrescriptionMedicines).WithOne(prescriptionMedicine => prescriptionMedicine.Prescription);

                entity.HasOne(prescription => prescription.PrescriptionUpload).WithOne(prescriptionUpload => prescriptionUpload.Prescription);
            });

            modelBuilder.Entity<DoctorActionLog>(entity =>
            {
                entity.ToTable("DoctorActionLog", "Doctor");

                entity.HasIndex(doctoractionlog => new { doctoractionlog.DoctorId, doctoractionlog.PrescriptionId })
                    .HasName("IDX_DoctorActionLog_PrescriptionID");

                entity.Property(doctoractionlog => doctoractionlog.DoctorActionLogId).HasColumnName("DoctorActionLogID");

                entity.Property(doctoractionlog => doctoractionlog.ActionTimeStamp).HasColumnType("datetime2(0)");

                entity.Property(doctoractionlog => doctoractionlog.DoctorId).HasColumnName("DoctorID");

                entity.Property(doctoractionlog => doctoractionlog.DoctorList)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(doctoractionlog => doctoractionlog.PrescriptionId).HasColumnName("PrescriptionID");

                entity.HasOne(doctoractionlog => doctoractionlog.Doctor)
                    .WithMany(doctor => doctor.DoctorActionLogs)
                    .HasForeignKey(doctoractionlog => doctoractionlog.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DoctorActionLog_Doctor");

                entity.HasOne(doctoractionlog => doctoractionlog.Prescription)
                    .WithMany(prescription => prescription.DoctorActionLogs)
                    .HasForeignKey(doctoractionlog => doctoractionlog.PrescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DoctorActionLog_Prescription");
            });

            modelBuilder.Entity<PrescriptionUpload>(entity =>
            {
                entity.Property(prescription => prescription.PrescriptionUploadId).HasColumnName("PrescriptionUploadID").HasColumnType("smallint").HasConversion(smallintToIntConverter);
                entity.HasOne(p => p.Prescription).WithOne(pu => pu.PrescriptionUpload);
            
                entity.Property(prescription => prescription.PrescriptionUploadId)
                    .HasColumnName("PrescriptionUploadID")
                    .HasColumnType("smallint")
                    .HasConversion(smallintToIntConverter);

                entity.Property(prescription => prescription.PrescriptionFileName)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(prescription => prescription.PrescriptionFilePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(prescription => prescription.IsDisable)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.Diseases)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(prescription => prescription.PrescriptionId)
                    .HasColumnName("PrescriptionId")
                    .HasColumnType("smallint")
                    .HasConversion(smallintToIntConverter);
            });
        }
    }
}
