import os

# 获取当前脚本所在目录
script_dir = os.path.dirname(os.path.abspath(__file__))

# 构建文本文件的完整路径
file_path = os.path.join(script_dir, '__hideoutcookconv.txt')

print(f"正在读取文件: {file_path}")
print("二进制内容:")

# 以二进制模式读取文件
with open(file_path, 'rb') as f:
    # 读取所有二进制数据
    binary_data = f.read()
    
    # 打印原始二进制数据（字节形式）
    print("原始字节序列:")
    print(binary_data)
    
    # 以十六进制格式打印（更易读）
    print("\n十六进制格式:")
    # 每16个字节打印一行，便于查看
    for i in range(0, len(binary_data), 16):
        chunk = binary_data[i:i+16]
        # 打印偏移量
        print(f"{i:08x}: ", end='')
        # 打印每个字节的十六进制表示
        hex_values = ' '.join(f"{byte:02x}" for byte in chunk)
        print(f"{hex_values: <47}", end='')  # 左对齐，固定宽度
        # 打印对应的ASCII字符（可打印的显示，不可打印的显示为.）
        ascii_chars = ''.join(chr(byte) if 32 <= byte <= 126 else '.' for byte in chunk)
        print(f"  {ascii_chars}")

print("\n读取完成!")