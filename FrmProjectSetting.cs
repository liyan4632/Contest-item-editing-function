using CCWin;
using SpeedSkating.DbServices;
using SpeedSkating.Models;
using SRSCMS.UI.CommClass;
using SRSCMS.UI.CommConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRSCMS.UI.Forms
{
    public partial class FrmProjectSetting : Skin_Color
    {
        public FrmProjectSetting()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmProjectSetting_Load(object sender, EventArgs e)
        {
            DGVProjectListDataSource();
            dgvCompetitionItemListDataSource();
        }
        /// <summary>
        /// 标准项目列表数据源
        /// </summary>
        private void DGVProjectListDataSource()
        {
            List<CompetitionItemList> itmeList = DotNet.Utilities.XmlHelper_Generic<CompetitionItemList>.GetListByXml("CommConfig\\CompetitionItem.xml");
            this.dgvStandardProjectList.DataSource = itmeList;
        }
        /// <summary>
        /// 全选复选框CheckedChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dgvStandardProjectList.RowCount; i++)
            {
                this.dgvStandardProjectList.Rows[i].Cells["IsSelected"].Value = this.cbxCheckAll.Checked;
            }
        }
        /// <summary>
        /// 添加按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string groupName = this.txtGroupName.Text;
            if (string.IsNullOrEmpty(groupName))
            {
                MessageBox.Show("请输入组别名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            List<Projectinfo> proList = new List<Projectinfo>();
            int groupId = GroupInfoServices.Instance.GetGourpIdByName(CommParam.PublicParam.GlobalMatchId, groupName);
            int maxSortIndex= ProjectInfoServices.Instance.MaxSortIndex(CommParam.PublicParam.GlobalMatchId);
            for (int i = 0; i < this.dgvStandardProjectList.Rows.Count; i++)
            {
                if (Convert.ToBoolean(this.dgvStandardProjectList.Rows[i].Cells["IsSelected"].Value))
                {
                    maxSortIndex++;
                    Projectinfo pro = new Projectinfo();
                    pro.Distance = Convert.ToInt32(this.dgvStandardProjectList.Rows[i].Cells["Distance"].Value);
                    pro.DistanceForQualification = Convert.ToInt32(this.dgvStandardProjectList.Rows[i].Cells["DistanceForQualification"].Value);
                    pro.StartTime = this.dtpStartTime.Value;
                    pro.EndTime = this.dtpStartTime.Value;
                    pro.MatchId = CommParam.PublicParam.GlobalMatchId;
                    pro.GroupId = groupId;
                    pro.Sex = this.dgvStandardProjectList.Rows[i].Cells["Sex"].Value.ToString() == "男" ? 0 : 1;
                    pro.SortIndex = maxSortIndex;
                    if (!groupName.Equals("专业"))
                    {
                        pro.ProjectName = this.dgvStandardProjectList.Rows[i].Cells["ProName"].Value.ToString().Insert(2, groupName);
                    }
                    else
                    {
                        pro.ProjectName = this.dgvStandardProjectList.Rows[i].Cells["ProName"].Value.ToString();
                    }
                    pro.TypeId = 1;
                    pro.EnglishProjectName = this.dgvStandardProjectList.Rows[i].Cells["EnglishName"].Value.ToString();
                    proList.Add(pro);
                }
            }
            if (proList.Count > 0)
            {
                if (ProjectInfoServices.Instance.Add(proList) > 0)
                {
                    dgvCompetitionItemListDataSource();
                    for (int i = 0; i < this.dgvStandardProjectList.Rows.Count; i++)
                    {
                        this.dgvStandardProjectList.Rows[i].Cells["IsSelected"].Value = 0;
                    }
                    this.cbxCheckAll.Checked = false;
                    this.txtGroupName.Text = "";
                }
            }
            else
            {
                MessageBox.Show("请选择项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        /// <summary>
        /// 比赛项目列表数据源
        /// </summary>
        private void dgvCompetitionItemListDataSource()
        {
            this.dgvEvents.DataSource = ProjectInfoServices.Instance.GetTableByMatchId(CommParam.PublicParam.GlobalMatchId);
        }
        /// <summary>
        /// 上移下移按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveBtnClick(object sender, EventArgs e)
        {
            int upOrDown = Convert.ToInt32((sender as Button).Tag);
            if (dgvEvents.SelectedRows.Count == 1)
            {
                int index = dgvEvents.SelectedRows[0].Index;//选中行索引
                int nextIndex = index + upOrDown;//目标行索引
                if (nextIndex >= 0 && nextIndex < dgvEvents.Rows.Count)
                {
                    string groupName = dgvEvents.Rows[nextIndex].Cells["GroupName"].Value.ToString();
                    string distance = dgvEvents.Rows[nextIndex].Cells["Distance_E"].Value.ToString();
                    string projectName = dgvEvents.Rows[nextIndex].Cells["ProjectName"].Value.ToString();
                    string startTime = dgvEvents.Rows[nextIndex].Cells["StartTime"].Value.ToString();
                    string projectId = dgvEvents.Rows[nextIndex].Cells["ProjectId"].Value.ToString();
                    dgvEvents.Rows[nextIndex].Cells["GroupName"].Value = dgvEvents.Rows[index].Cells["GroupName"].Value;
                    dgvEvents.Rows[nextIndex].Cells["Distance_E"].Value = dgvEvents.Rows[index].Cells["Distance_E"].Value;
                    dgvEvents.Rows[nextIndex].Cells["ProjectName"].Value = dgvEvents.Rows[index].Cells["ProjectName"].Value;
                    dgvEvents.Rows[nextIndex].Cells["StartTime"].Value = dgvEvents.Rows[index].Cells["StartTime"].Value;
                    dgvEvents.Rows[nextIndex].Cells["ProjectId"].Value = dgvEvents.Rows[index].Cells["ProjectId"].Value;
                    dgvEvents.Rows[index].Cells["GroupName"].Value = groupName;
                    dgvEvents.Rows[index].Cells["Distance_E"].Value = distance;
                    dgvEvents.Rows[index].Cells["ProjectName"].Value = projectName;
                    dgvEvents.Rows[index].Cells["StartTime"].Value = startTime;
                    dgvEvents.Rows[index].Cells["ProjectId"].Value = projectId;
                    dgvEvents.Rows[index].Selected = false;
                    dgvEvents.Rows[nextIndex].Selected = true;
                }
            }
        }
        /// <summary>
        /// 保存按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckChange())
            {
                SaveChange();
            }
            else
            {
                MessageBox.Show("无任何修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        /// <summary>
        /// 保存更改
        /// </summary>
        private void SaveChange()
        {
            List<Projectinfo> proList = new List<Projectinfo>();
            foreach (DataGridViewRow item in dgvEvents.Rows)
            {
                Projectinfo pro = new Projectinfo()
                {
                    ProjectId = Convert.ToInt32(item.Cells["ProjectId"].Value),
                    ProjectName = item.Cells["ProjectName"].Value.ToString(),
                    StartTime = Convert.ToDateTime(item.Cells["StartTime"].Value),
                    SortIndex = Convert.ToInt32(item.Cells["SortIndex"].Value)
                };
                proList.Add(pro);
            }
            if (proList.Count > 0)
            {
                if (ProjectInfoServices.Instance.Update(proList) > 0)
                {
                    MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("保存失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }
        /// <summary>
        /// 检查是否有更改
        /// </summary>
        /// <returns></returns>
        public bool CheckChange()
        {
            bool change = false;
            List<Projectinfo> dgvSort = new List<Projectinfo>();
            foreach (DataGridViewRow item in dgvEvents.Rows)
            {
                Projectinfo pro = new Projectinfo()
                {
                    ProjectId = Convert.ToInt32(item.Cells["ProjectId"].Value),
                    ProjectName = item.Cells["ProjectName"].Value.ToString(),
                    StartTime = Convert.ToDateTime(item.Cells["StartTime"].Value)
                };
                dgvSort.Add(pro);
            }
            DataTable dt= ProjectInfoServices.Instance.GetTableByMatchId(CommParam.PublicParam.GlobalMatchId);
            if (dgvSort.Count == dt.Rows.Count)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dgvSort[i].ProjectId != Convert.ToInt32(dt.Rows[i]["ProjectId"])|| dgvSort[i].ProjectName!= dt.Rows[i]["ProjectName"].ToString()|| dgvSort[i].StartTime.ToString("yyyy/MM/dd hh:mm")!=Convert.ToDateTime(dt.Rows[i]["StartTime"]).ToString("yyyy/MM/dd hh:mm"))
                    {
                        change = true;
                        break;
                    }
                }
            }
            else
            {
                change = true;
            }
            return change;
        }
        /// <summary>
        /// 项目信息更改完成后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvEvents_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Projectinfo pro = new Projectinfo();
            string proName = this.dgvEvents.Rows[e.RowIndex].Cells["ProjectName"].Value.ToString();
            string strStartTime= this.dgvEvents.Rows[e.RowIndex].Cells["StartTime"].Value.ToString();
            DateTime dtStartTime;
            if (string.IsNullOrEmpty(proName))
            {
                MessageBox.Show("项目名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (!DateTime.TryParse(strStartTime, out dtStartTime))
            {
                MessageBox.Show("请输入正确的时间格式，如2019/1/1 12:00", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
        }
        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmProjectSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CheckChange())
            {
                DialogResult dr= MessageBox.Show("是否保存更改？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    SaveChange();
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEvents.SelectedRows.Count == 0)
            {
                MessageBox.Show("没有选中任何项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (MessageBox.Show("确定删除所选项目？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                List<Projectinfo> proIdList = new List<Projectinfo>();
                foreach (DataGridViewRow item in dgvEvents.SelectedRows)
                {
                    Projectinfo pro = new Projectinfo()
                    {
                        ProjectId = Convert.ToInt32(item.Cells["ProjectId"].Value),
                        GroupId = Convert.ToInt32(item.Cells["GroupId"].Value)
                    };
                    proIdList.Add(pro);
                }
                ProjectInfoServices.Instance.Delete(proIdList, CommParam.PublicParam.GlobalMatchId);
                dgvCompetitionItemListDataSource();
            }
        }
        /// <summary>
        /// 积分点设置按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIntegrationPointSetting_Click(object sender, EventArgs e)
        {
            FrmIntegrationPointSetting fips = new FrmIntegrationPointSetting();
            fips.ShowDialog();
        }
    }
}
