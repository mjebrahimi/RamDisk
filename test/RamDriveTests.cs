using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace RamDisk.Tests
{
    public class RamDriveTests
    {
        private DriveInfo Drive;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            try { RamDrive.Unmount('X'); } catch { }
            try { RamDrive.Unmount('Y'); } catch { }

            TestDelegate action = () => RamDrive.Mount(128, FileSystem.NTFS, 'X', "MyDriveName");
            Assert.DoesNotThrow(action);

            Drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name.ToUpper()[0] == 'X');
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            try { RamDrive.Unmount('Y'); } catch { }

            TestDelegate action = () => RamDrive.Unmount('X');
            Assert.DoesNotThrow(action);

            var exists = DriveInfo.GetDrives().Any(d => d.Name.ToUpper()[0] == 'X');
            Assert.IsFalse(exists);
        }

        [Test]
        public void Drive_Should_Exists_And_Ready()
        {
            Assert.IsNotNull(Drive);
            Assert.IsTrue(Drive.IsReady);
        }

        [Test]
        public void Drive_Name_ShouldBe_Z()
        {
            Assert.AreEqual(Drive.Name, "X:\\");
        }

        [Test]
        public void Drive_Label_ShouldBe_RamDisk()
        {
            Assert.AreEqual(Drive.VolumeLabel, "MyDriveName");
        }

        [Test]
        public void Drive_Format_ShouldBe_NTFS()
        {
            Assert.AreEqual(Drive.DriveFormat, FileSystem.NTFS.ToString());
        }

        [Test]
        public void Drive_Size_ShouldBe_Correct()
        {
            decimal kb = 1024;
            var size = Math.Round(Drive.TotalSize / kb / kb);
            Assert.AreEqual(size, 128);
        }

        [Test]
        public void Duplicate_Drive_Name_Should_Throws_InvalidOperationException()
        {
            TestDelegate action = () => RamDrive.Mount(128, FileSystem.NTFS, 'X', "MyDriveName");
            Assert.Throws<InvalidOperationException>(action);
        }

        [Test]
        public void Zero_Drive_Size_Should_Throws_ArgumentException()
        {
            TestDelegate action = () => RamDrive.Mount(0);
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public void Negative_Drive_Size_Should_Throws_ArgumentException()
        {
            TestDelegate action = () => RamDrive.Mount(-128);
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public void Null_Drive_Label_Should_Throws_ArgumentNullException()
        {
            TestDelegate action = () => RamDrive.Mount(128, FileSystem.NTFS, 'X', null);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Test]
        public void WhiteSpace_Drive_Label_Should_Throws_ArgumentNullException()
        {
            TestDelegate action = () => RamDrive.Mount(128, FileSystem.NTFS, 'X', "");
            Assert.Throws<ArgumentNullException>(action);
        }

        [Test]
        public void Unmount_NotExists_Drive_Should_Throws_InvalidOperationException()
        {
            TestDelegate action = () => RamDrive.Unmount('Q');
            Assert.Throws<InvalidOperationException>(action);
        }

        [Test]
        [TestCase(FileSystem.exFAT)]
        [TestCase(FileSystem.FAT)]
        [TestCase(FileSystem.FAT32)]
        public void Another_Drive_Format_Should_Work_Correctly(FileSystem fileSystem)
        {
            TestDelegate action = () => RamDrive.Mount(128, fileSystem, 'Y', "RamDisk");
            Assert.DoesNotThrow(action);

            var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name.ToUpper()[0] == 'Y');
            Assert.IsNotNull(drive);

            Assert.AreEqual(drive.DriveFormat, fileSystem.ToString());

            TestDelegate action2 = () => RamDrive.Unmount('Y');
            Assert.DoesNotThrow(action2);

            Thread.Sleep(100);

            var exists = DriveInfo.GetDrives().Any(d => d.Name.ToUpper()[0] == 'Y');
            Assert.IsFalse(exists);
        }
    }
}