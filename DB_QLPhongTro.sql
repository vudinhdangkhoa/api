create database QLPhongTro
go
use QLPhongTro
go

create table Chu(
	idChu int primary key identity(1,1),
	ten nvarchar(50)not null,
	taiKhoan varchar(30) not null,
	matKhau varchar(30) not null,
)

create table CoSo(
	idCoSo int primary key identity(1,1),
	tenCoSo nvarchar(50)not null ,
	diaChi nvarchar(255) not null,
	idChu int not null,
	foreign key (idChu) references Chu(idChu)
)

create table Phong(
	idPhong int primary key identity(1,1),
	tenPhong nvarchar(50) ,
	soLuong int default 0 not null,
	idCoSo int not null,
	foreign key (idCoSo) references CoSo(idCoSo)
)

create table KhachHang(
	idKH int primary key identity(1,1),
	tenKH nvarchar(255),
	avatar varchar(255),
	sdt char(10) not null,
	cccd char(12) not null,
	ngayDen date default getdate(),
	ngayDi date ,
	tinhtrang int,--0 la roi di, 1 la dang o
	email varchar(50),
	matKhau varchar(30) default '123' not null,
	idPhong int not null,
	foreign key (idPhong) references Phong(idPhong)
)

create table HoaDon(
	idHoaDon int primary key identity(1,1),
	soTien float,
	idPhong int not null,
	foreign key (idPhong) references Phong(idPhong)
)

create table Chat(
	idChat int primary key identity(1,1),
	idKH int not null,
	nameChat nvarchar(50) not null,
	avatar varchar(255) ,

)

create table Message(
	idMess int primary key identity(1,1),
	nguoiGui nvarchar(50) not null,
	noiDung nvarchar(Max) ,
	idChat int,
	foreign key (idChat) references Chat(idChat)
)

go


CREATE TRIGGER trg_InsertKhachHang
ON KhachHang
AFTER INSERT
AS
BEGIN
    
	declare @sl int
	select @sl= count(*) from inserted
    UPDATE Phong
    SET soLuong = soLuong + @sl
    FROM Phong p
    INNER JOIN inserted i ON p.idPhong = i.idPhong;

   
    INSERT INTO Chat (idKH, nameChat, avatar)
    SELECT idKH, tenKH, avatar
    FROM inserted;
END;
go

create trigger trg_deleteKhachHang
on KhachHang
after delete
as
begin
	declare @sl int
	select @sl=count(*)from deleted
	update Phong
	SET soLuong = soLuong - @sl
    FROM Phong p
    INNER JOIN deleted i ON p.idPhong = i.idPhong;
end
go

create trigger trg_deletePhong
on Phong
after delete
as
begin
	declare @sl int
	select @sl=count(*)from deleted
	update CoSo
	SET soLuong = c.soLuong - @sl
    FROM CoSo c
    INNER JOIN deleted i ON c.idCoSo = i.idCoSo;
end
go


ALTER TABLE Chat 
ADD CONSTRAINT FK_Chat_KhachHang FOREIGN KEY (idKH) REFERENCES KhachHang(idKH);

alter table HoaDon
add trangThai int default 0 --0 là chưa có trả tiền, 1 là đã trả tiền,  -1 là mới tạo chưa có tiền điện và nước, 2 là đang chờ xác nhận

alter table HoaDon
add ngayThanhToan date 

alter table HoaDon
add ngayTao date

alter table HoaDon
add tienDien float 

alter table HoaDon
add tienNuoc float

alter table HoaDon
add anhHoaDon varchar(255)

alter table CoSo
add  trangThai int default 1 --1 là cơ sở đó vẫn còn 0 là đã xóa cơ sở đó

alter table Phong
add  trangThai int default 1 --1 là phòng đó vẫn còn 0 là đã xóa phòng đó

alter table Phong
add tienPhong float 

alter table CoSo
add soLuong int default 0
go

alter table Chu
add avatar varchar(50) default 'khonghinh'
go 

alter table Chu
add giaNuoc float default 0
go

alter table Chu
add giaDien float default 0
go



--select * from HoaDon
--select * from KhachHang
select * from Chu
update HoaDon
set ngayThanhToan=null
where  idHoaDon=56
update HoaDon
set anhHoaDon=null
where idHoaDon=56
update HoaDon
set trangThai=0
where idHoaDon=56
update KhachHang
set avatar='khonghinh'
update HoaDon
set soTien=3000000
update Chu
set giaNuoc=0
where idChu=2
update Chu
set giaDien=0
where idChu=2


SELECT 
    MONTH(hd.ngayThanhToan) AS Thang,
    YEAR(hd.ngayThanhToan) AS Nam,
    SUM(hd.soTien) AS TongTien
FROM HoaDon hd
JOIN Phong p ON hd.idPhong = p.idPhong
JOIN CoSo cs ON p.idCoSo = cs.idCoSo
JOIN Chu chu ON chu.idChu = cs.idChu
WHERE hd.trangThai = 1
GROUP BY YEAR(hd.ngayThanhToan), MONTH(hd.ngayThanhToan)
ORDER BY Nam, Thang;



create trigger trg_insertPhong
on Phong
after insert
as
begin

	declare @sl int
	select @sl= count(*) from inserted
	update s
	set s.soLuong=s.soLuong+@sl
	from CoSo s
	inner join inserted i on s.idCoSo=i.idCoSo
end

CREATE PROCEDURE TaoHoaDonTuDong
    @idChu INT
AS
BEGIN
    DECLARE @idPhong INT, @idCoSo INT,@tienPhong float
   

   
    DECLARE PhongCursor CURSOR FOR
    SELECT p.tienPhong, p.idPhong, p.idCoSo FROM Phong p
    JOIN CoSo c ON p.idCoSo = c.idCoSo
    WHERE c.idChu = @idChu AND c.trangThai = 1 AND p.trangThai = 1 and p.soLuong>0

   
    OPEN PhongCursor
    FETCH NEXT FROM PhongCursor INTO @idPhong, @idCoSo

    WHILE @@FETCH_STATUS = 0
    BEGIN
       
        INSERT INTO HoaDon (soTien, idPhong, trangThai, ngayThanhToan, tienDien, tienNuoc,ngayTao)
        VALUES (@tienPhong, @idPhong, -1, null, 0, 0,GETDATE())

       
        FETCH NEXT FROM PhongCursor INTO @idPhong, @idCoSo
    END

  
    CLOSE PhongCursor
    DEALLOCATE PhongCursor
END;

--exec TaoHoaDonTuDong  @idChu=1 

--delete HoaDon

--select *from HoaDon 
--where ngayTao= '2025-06-05'

--select * from Phong where idCoSo=2

--INSERT INTO Phong (tenPhong, idCoSo) 
--VALUES 
--(N'Phòng A1', 4),
--(N'Phòng B2', 4);
--INSERT INTO Phong (tenPhong, idCoSo) 
--VALUES
--(N'Phòng C1', 4)

--INSERT INTO KhachHang (tenKH, avatar, sdt, cccd, ngayDen, ngayDi, tinhtrang, email, matKhau, idPhong) 
--VALUES 
--(N'Nguyễn Văn A', 'avatar1.jpg', '0123456789', '123456789012', GETDATE(), NULL, 1, 'nguyenvana@example.com', '123', 1),
--(N'Trần Thị B', 'avatar2.jpg', '0987654321', '987654321098', GETDATE(), NULL, 1, 'tranthib@example.com', '123', 1),
--(N'Lê Văn C', 'avatar3.jpg', '0369852147', '369852147369', GETDATE(), NULL, 1, 'levanc@example.com', '123', 1);

--INSERT INTO KhachHang (tenKH, avatar, sdt, cccd, ngayDen, ngayDi, tinhtrang, email, matKhau, idPhong) 
--VALUES 
--(N'Nguyễn Văn D', 'avatar4.jpg', '0354789652', '354789652354', GETDATE(), NULL, 1, 'nguyenvand@example.com', '123', 3),
--(N'Trần Thị E', 'avatar5.jpg', '0963254789', '963254789963', GETDATE(), NULL, 1, 'tranthie@example.com', '123', 3),
--(N'Lê Văn F', 'avatar6.jpg', '0321456987', '321456987321', GETDATE(), NULL, 1, 'levanf@example.com', '123', 3);

--INSERT INTO KhachHang (tenKH, avatar, sdt, cccd, ngayDen, ngayDi, tinhtrang, email, matKhau, idPhong) 
--VALUES 
--(N'Phạm Văn G', 'avatar7.jpg', '0345698712', '345698712345', GETDATE(), NULL, 1, 'phamvang@example.com', '123', 4),
--(N'Nguyễn Thị H', 'avatar8.jpg', '0978456321', '978456321978', GETDATE(), NULL, 1, 'nguyenthih@example.com', '123', 4),
--(N'Lê Thành I', 'avatar9.jpg', '0397854123', '397854123397', GETDATE(), NULL, 1, 'lethanh@example.com', '123', 4);

--INSERT INTO KhachHang (tenKH, avatar, sdt, cccd, ngayDen, ngayDi, tinhtrang, email, matKhau, idPhong) 
--VALUES 
--(N'Nguyễn Văn J', 'avatar10.jpg', '0365987412', '365987412365', GETDATE(), NULL, 1, 'nguyenvanj@example.com', '123', 5),
--(N'Trần Thị K', 'avatar11.jpg', '0912345678', '912345678912', GETDATE(), NULL, 1, 'tranthik@example.com', '123', 5),
--(N'Lê Văn L', 'avatar12.jpg', '0387654123', '387654123387', GETDATE(), NULL, 1, 'levanl@example.com', '123', 5);

--INSERT INTO HoaDon (soTien, idPhong, trangThai, ngayThanhToan, ngayTao, tienDien, tienNuoc)  
--VALUES
--(500000, 1, 1, '2025-06-02', '2025-06-01', 100000, 3*60000),
--(500000, 1, 1, '2025-05-02', '2025-05-01', 90000, 3*60000),
--(500000, 1, 1, '2025-04-02', '2025-04-01', 120000, 3*60000),
--(500000, 1, 1, '2025-03-02', '2025-03-01', 110000, 3*60000),
--(500000, 1, 1, '2025-02-02', '2025-02-01', 96000, 3*60000),

--(500000, 3, 1, '2025-06-02', '2025-06-01', 120000, 3*60000),
--(500000, 3, 1, '2025-05-02', '2025-05-01', 116000, 3*60000),
--(500000, 3, 1, '2025-04-02', '2025-04-01', 124000, 3*60000),
--(500000, 3, 1, '2025-03-02', '2025-03-01', 118000, 3*60000),
--(500000, 3, 1, '2025-02-02', '2025-02-01', 112000, 3*60000),

--(500000, 4, 1, '2025-06-02', '2025-06-01', 160000, 3*60000),
--(500000, 4, 1, '2025-05-02', '2025-05-01', 170000, 3*60000),
--(500000, 4, 1, '2025-04-02', '2025-04-01', 164000, 3*60000),
--(500000, 4, 1, '2025-03-02', '2025-03-01', 156000, 3*60000),
--(500000, 4, 1, '2025-02-02', '2025-02-01', 150000, 3*60000),

--(500000, 5, 1, '2025-06-02', '2025-06-01', 180000, 3*60000),
--(500000, 5, 1, '2025-05-02', '2025-05-01', 184000, 3*60000),
--(500000, 5, 1, '2025-04-02', '2025-04-01', 176000, 3*60000),
--(500000, 5, 1, '2025-03-02', '2025-03-01', 172000, 3*60000),
--(500000, 5, 1, '2025-02-02', '2025-02-01', 178000, 3*60000);




