using Lab05.BUS;
using Lab05.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05.GUI
{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudent = studentService.GetAll();
                FillFacultyCombobox(listFacultys);
                BindGrid(listStudent);

            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void setGridViewStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.BackgroundColor = Color.White;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void FillFacultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach(var item in listStudents){
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if(item.Faculty!= null)
                {
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                    dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + "";

                }
                if(item.Major != null)
                {
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name + "";
                    ShowAvatar(item.Avatar);
                }

            }
        }
        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                picAvatar.Image = null;
            }
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", ImageName);
                picAvatar.Image = Image.FromFile(imagePath);
                picAvatar.Refresh();
            }
        }

        private void chkChuyenNganh_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkChuyenNganh.Checked)
            {
                listStudents = studentService.GetAllHasNoMajor();
            }
            else
            {
                listStudents = studentService.GetAll();
            }
            BindGrid(listStudents);
        }

        private void dKChuyenNganhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                StudentModel studentModel = new StudentModel();
                Faculty selectedFacultyObj = studentModel.Faculties.FirstOrDefault(f => f.FacultyName == cmbFaculty.Text);

                string avatarPath;
                if(picAvatar.Image == null)
                {
                    avatarPath = "";
                }
                else
                {
                    avatarPath = txtStudentID.Text;

                    // Tạo biến để lưu trữ đường dẫn đến folder đích
                    string imagesDirectoryPath = "D:\\code\\MS Visual Studio\\C#\\lab05_TH\\lab05\\Lab05\\Lab05.GUI\\Images";


                    // Tạo biến để lưu trữ dữ liệu của file ảnh trong picturebox
                    Image image = picAvatar.Image;

                    // Lưu file ảnh vào folder đích
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Image files (*.png, *.jpg, *.gif)|*.png;*.jpg;*.gif";
                    saveDialog.InitialDirectory = imagesDirectoryPath;
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Lưu file ảnh vào folder đích
                        string imagePath = Path.Combine(imagesDirectoryPath, avatarPath);
                        string filetype = Path.GetExtension(avatarPath);

                        // Lưu file ảnh
                        picAvatar.Image.Save(imagePath+filetype);
                        avatarPath += filetype;
                    }
                    


                }


                Student std = new Student() { StudentID = txtStudentID.Text, FullName = txtName.Text, AverageScore = double.Parse(txtAverageScore.Text), FacultyID = selectedFacultyObj.FacultyID, Avatar = avatarPath };

                if(studentService.FindByID(txtStudentID.Text) == null)
                {
                    studentService.InsertUpdate(std);
                    throw new Exception("Thêm mới thành công !");
                }
                else
                {
                    studentService.InsertUpdate(std);   
                    throw new Exception("Cập nhật thành công !");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            var listStudent = studentService.GetAll();
            BindGrid(listStudent);
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult dl = MessageBox.Show("Bạn có muốn thoát ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dl == DialogResult.Yes) { 
                this.Close();
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int index = e.RowIndex;

            txtStudentID.Text = dgvStudent.Rows[index].Cells[0].Value.ToString();
            txtName.Text = dgvStudent.Rows[index].Cells[1].Value.ToString();
            cmbFaculty.Text = dgvStudent.Rows[index].Cells[2].Value.ToString();
            txtAverageScore.Text = dgvStudent.Rows[index].Cells[3].Value.ToString();

            if (studentService.FindByID(txtStudentID.Text).Avatar !=null)
            {

                ShowAvatar(studentService.FindByID(txtStudentID.Text).Avatar);
            }
            else
            {
                picAvatar.Image = null;
            }


        }

        private void btnAddPic_Click(object sender, EventArgs e)
        {
            // Tạo đối tượng OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Cài đặt các thuộc tính của OpenFileDialog
            openFileDialog.Filter = "Hình ảnh (*.jpg; *.png; *.gif)|*.jpg;*.png;*.gif";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;

            // Mở tệp hình ảnh
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Lưu đường dẫn đến tệp hình ảnh
                string imagePath = openFileDialog.FileName;

                // Tạo đối tượng Image
                Image image = Image.FromFile(imagePath);

                // Đặt hình ảnh cho pictureBox
                picAvatar.Image = image;
            }
        }
    }
}
