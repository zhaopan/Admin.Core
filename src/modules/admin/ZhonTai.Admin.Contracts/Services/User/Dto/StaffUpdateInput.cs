﻿
namespace ZhonTai.Admin.Domain.User;

/// <summary>
/// 员工修改
/// </summary>
public class StaffUpdateInput: StaffAddInput
{
    /// <summary>
    /// 编号
    /// </summary>
    public long Id { get; set; }
}