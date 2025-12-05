import { useEffect, useState } from 'react';
import { Table, Tag, Typography, Card, Button, Modal, Form, Input, Select, message, Popconfirm, Tabs, InputNumber, Alert } from 'antd';
import axios from 'axios';
import { UserOutlined, ReloadOutlined, PlusOutlined, DeleteOutlined, HomeOutlined, TeamOutlined } from '@ant-design/icons';

const { Title } = Typography;
const { Option } = Select;

// --- CẤU HÌNH API (Cổng 5094 như bạn đã báo) ---
const PORT = 5094; 
const API_STUDENTS = `http://localhost:${PORT}/api/Students`;
const API_BUILDINGS = `http://localhost:${PORT}/api/Buildings`;

function App() {
  // --- STATE QUẢN LÝ TAB ---
  const [activeTab, setActiveTab] = useState('1'); // 1: Sinh viên, 2: Tòa nhà

  // ==============================
  // 1. LOGIC QUẢN LÝ SINH VIÊN (Code cũ của bạn)
  // ==============================
  const [students, setStudents] = useState([]);
  const [loadingSV, setLoadingSV] = useState(false);
  const [modalSV, setModalSV] = useState(false);
  const [formSV] = Form.useForm();

  const fetchStudents = async () => {
    setLoadingSV(true);
    try { const res = await axios.get(API_STUDENTS); setStudents(res.data); } 
    catch (e) { message.error('Lỗi tải dữ liệu sinh viên!'); } 
    finally { setLoadingSV(false); }
  };

  const handleAddStudent = async (values) => {
    try { 
      await axios.post(API_STUDENTS, values); 
      message.success('Thêm sinh viên thành công!'); 
      setModalSV(false); formSV.resetFields(); fetchStudents(); 
    } 
    catch (e) { message.error('Lỗi khi thêm sinh viên!'); }
  };

  const handleDeleteSV = async (id) => {
    try { await axios.delete(`${API_STUDENTS}/${id}`); message.success('Đã xóa sinh viên!'); fetchStudents(); } 
    catch (e) { message.error('Không thể xóa sinh viên này!'); }
  };

  // ==============================
  // 2. LOGIC QUẢN LÝ TÒA NHÀ (Mới thêm vào)
  // ==============================
  const [buildings, setBuildings] = useState([]);
  const [loadingBuild, setLoadingBuild] = useState(false);
  const [modalBuild, setModalBuild] = useState(false);
  const [formBuild] = Form.useForm();

  const fetchBuildings = async () => {
    setLoadingBuild(true);
    try { const res = await axios.get(API_BUILDINGS); setBuildings(res.data); } 
    catch (e) { message.error('Lỗi tải dữ liệu tòa nhà!'); } 
    finally { setLoadingBuild(false); }
  };

  const handleAddBuilding = async (values) => {
    try { 
      // Hiển thị thông báo đang xử lý vì tạo 300 phòng sẽ mất khoảng 1-2 giây
      message.loading({ content: 'Đang khởi tạo tòa nhà và phòng...', key: 'create_build' });
      
      await axios.post(API_BUILDINGS, values); 
      
      message.success({ content: 'Tạo tòa nhà & phòng thành công!', key: 'create_build' });
      setModalBuild(false); formBuild.resetFields(); fetchBuildings(); 
    } 
    catch (e) { message.error({ content: 'Lỗi tạo tòa nhà!', key: 'create_build' }); }
  };

  const handleDeleteBuilding = async (id) => {
    try { await axios.delete(`${API_BUILDINGS}/${id}`); message.success('Đã xóa tòa nhà!'); fetchBuildings(); } 
    catch (e) { message.error('Không xóa được tòa nhà!'); }
  };

  // --- EFFECT: Tải dữ liệu khi chuyển Tab ---
  useEffect(() => {
    if (activeTab === '1') fetchStudents();
    if (activeTab === '2') fetchBuildings();
  }, [activeTab]);

  // --- CỘT BẢNG SINH VIÊN ---
  const columnsSV = [
    { title: 'Mã SV', dataIndex: 'studentCode', key: 'studentCode', render: t => <b>{t}</b> },
    { title: 'Họ tên', dataIndex: 'fullName', key: 'fullName', render: t => <span><UserOutlined style={{ marginRight: 8 }} />{t}</span> },
    { title: 'Email', dataIndex: 'email', key: 'email' },
    { title: 'SĐT', dataIndex: 'phoneNumber', key: 'phoneNumber' },
    { title: 'Giới tính', dataIndex: 'gender', key: 'gender', render: g => <Tag color={g==='Nam'?'blue':(g==='Nữ'?'pink':'geekblue')}>{g||'Khác'}</Tag> },
    { title: '', key: 'action', render: (_, r) => <Popconfirm title="Xóa?" onConfirm={() => handleDeleteSV(r.id)}><Button danger icon={<DeleteOutlined/>} size='small'/></Popconfirm> }
  ];

  // --- CỘT BẢNG TÒA NHÀ ---
  const columnsBuild = [
    { title: 'Tên Tòa Nhà', dataIndex: 'name', key: 'name', render: t => <b style={{fontSize: 16, color: '#1890ff'}}>{t}</b> },
    { title: 'Số tầng', dataIndex: 'totalFloors', key: 'totalFloors', render: t => <Tag color="geekblue">{t} Tầng</Tag> },
    { title: 'Hành động', key: 'action', render: (_, r) => <Popconfirm title="Xóa tòa này sẽ mất hết phòng?" onConfirm={() => handleDeleteBuilding(r.id)}><Button danger icon={<DeleteOutlined/>}>Xóa</Button></Popconfirm> }
  ];

  // ==============================
  // GIAO DIỆN CHÍNH (RENDER)
  // ==============================
  return (
    <div style={{ padding: '40px', background: '#f0f2f5', minHeight: '100vh' }}>
      <Card style={{ borderRadius: 12, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
        <Title level={2} style={{ textAlign: 'center', marginBottom: 30 }}>🎓 QUẢN LÝ KÝ TÚC XÁ</Title>
        
        {/* THANH TAB CHUYỂN ĐỔI */}
        <Tabs 
          activeKey={activeTab} 
          onChange={setActiveTab}
          type="card"
          items={[
            {
              key: '1',
              label: <span><TeamOutlined />Quản lý Sinh Viên</span>,
              children: (
                <>
                  <div style={{ marginBottom: 16, display: 'flex', gap: 10 }}>
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalSV(true)}>Thêm Sinh viên</Button>
                    <Button icon={<ReloadOutlined />} onClick={fetchStudents}>Tải lại</Button>
                  </div>
                  <Table dataSource={students} columns={columnsSV} rowKey="id" loading={loadingSV} bordered pagination={{ pageSize: 5 }} />
                </>
              ),
            },
            {
              key: '2',
              label: <span><HomeOutlined />Quản lý Tòa Nhà & Phòng</span>,
              children: (
                <>
                   <div style={{ marginBottom: 16, display: 'flex', gap: 10 }}>
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalBuild(true)} style={{ background: '#52c41a', borderColor: '#52c41a' }}>
                      Thêm Tòa Nhà Mới
                    </Button>
                    <Button icon={<ReloadOutlined />} onClick={fetchBuildings}>Tải lại</Button>
                  </div>
                  <Table dataSource={buildings} columns={columnsBuild} rowKey="id" loading={loadingBuild} bordered />
                </>
              ),
            },
          ]}
        />
      </Card>

      {/* --- MODAL 1: THÊM SINH VIÊN (Đã cập nhật theo ý bạn) --- */}
      <Modal title="Thêm Sinh viên mới" open={modalSV} onCancel={() => setModalSV(false)} footer={null}>
        <Form form={formSV} layout="vertical" onFinish={handleAddStudent}>
          <Form.Item label="Mã Sinh viên" name="studentCode" rules={[{ required: true, message: 'Vui lòng nhập mã SV!' }]}>
            <Input placeholder="" /> 
          </Form.Item>
          
          <Form.Item label="Họ và tên" name="fullName" rules={[{ required: true, message: 'Vui lòng nhập họ tên!' }]}>
            <Input placeholder="" />
          </Form.Item>

          <Form.Item label="Email" name="email" rules={[{ type: 'email', message: 'Email không hợp lệ!' }]}>
            <Input placeholder="abc@gmail.com" />
          </Form.Item>

          <Form.Item label="Số điện thoại" name="phoneNumber">
            <Input placeholder="" />
          </Form.Item>

          <Form.Item label="Giới tính" name="gender">
            <Select placeholder="Chọn giới tính">
              <Option value="Nam">Nam</Option>
              <Option value="Nữ">Nữ</Option>
            </Select>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" style={{ width: '100%' }}>Lưu thông tin</Button>
          </Form.Item>
        </Form>
      </Modal>

      {/* --- MODAL 2: THÊM TÒA NHÀ & TỰ ĐỘNG SINH PHÒNG --- */}
      <Modal title="🏠 Khởi tạo Tòa Nhà & Phòng" open={modalBuild} onCancel={() => setModalBuild(false)} footer={null}>
        <Form form={formBuild} layout="vertical" onFinish={handleAddBuilding} initialValues={{ totalFloors: 5, roomsPerFloor: 10 }}>
          
          <Form.Item name="name" label="Tên Tòa Nhà" rules={[{ required: true, message: 'Nhập tên tòa!' }]}>
            <Input placeholder="Ví dụ: Tòa A1" size="large" />
          </Form.Item>

          <div style={{ display: 'flex', gap: 20 }}>
            <Form.Item name="totalFloors" label="Số Tầng" style={{ flex: 1 }} rules={[{ required: true }]}>
              <InputNumber min={1} max={50} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="roomsPerFloor" label="Số phòng / tầng" style={{ flex: 1 }} rules={[{ required: true }]}>
              <InputNumber min={1} max={50} style={{ width: '100%' }} />
            </Form.Item>
          </div>

          <Alert 
            message="Hệ thống sẽ tự động tạo phòng" 
            description="Ví dụ: 15 tầng x 20 phòng = 300 phòng được tạo tự động." 
            type="info" showIcon style={{ marginBottom: 20 }} 
          />

          <Button type="primary" htmlType="submit" block style={{ height: 40, background: '#52c41a' }}>
            🚀 Tạo ngay
          </Button>
        </Form>
      </Modal>
    </div>
  );
}

export default App;