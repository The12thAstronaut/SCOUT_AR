%fileID = fopen("LDEM_128.IMG", 'r', 'ieee-le');
%fileID = fopen('Site01_final_adj_5mpp_surf.tif', 'r', 'n')
%if fileID == -1, error('Cannot open file: %s', filename); end
[Data, R] = readgeoraster('Site01_final_adj_5mpp_surf.tif');
format = "int16";
width = 3200;
height = 3200;
%min = 18255;
%maxVal = 21557 + min;
range = 523.1834106445312 + 1959.496215820312

%Data = fread(fileID, [width, height], format);
%fclose(fileID);
maxHeight = max(Data, [], 'all')
minHeight = min(Data, [], 'all')
%maxHeight = 1959.496215820312;
%minHeight = -523.1834106445312;
whos Data
disp(Data(500, 100));

%newData = int16(Data);
%newData = single((Data + min) / maxVal);
newData = uint16(((Data - minHeight) / (maxHeight - minHeight))*65535);
%newData = im2uint16(Data / maxVal);

%newData = single(Data + min);
%disp(newData(500, 100));
%disp(newData(500, 100) * (maxVal / 1));
%disp(max(newData, [], "all"));
%disp(min(newData, [], "all"));
whos newData
%fileID = fopen('Corrected_LDEM_128.tif','w');
%fwrite(fileID,newData,'uint16');
imwrite(newData, "Corrected_Site01_final_adj_5mpp_surf.tif", "tif", "Compression", "none");

%{
t = Tiff("test.tif", 'w');
setTag(t,'ImageLength', 500);
setTag(t,'ImageWidth', 500);
setTag(t,'Photometric', Tiff.Photometric.MinIsBlack);
setTag(t,'PlanarConfiguration', Tiff.PlanarConfiguration.Chunky);
setTag(t, 'SampleFormat', Tiff.SampleFormat.IEEEFP);
setTag(t,'BitsPerSample', 32);
setTag(t,'SamplesPerPixel', 1);
setTag(t, 'Compression', Tiff.Compression.None);
write(t, single(rand(500)));
close(t);


t = Tiff("Corrected_LDEM_128.tif", 'w');
setTag(t,'ImageLength', size(newData, 1) / 2);
setTag(t,'ImageWidth', size(newData, 2) / 4);
setTag(t,'Photometric', Tiff.Photometric.MinIsBlack);
setTag(t,'PlanarConfiguration', Tiff.PlanarConfiguration.Chunky);
setTag(t,'BitsPerSample', 16);
setTag(t,'SamplesPerPixel', 1);
%setTag(t, 'SampleFormat', Tiff.SampleFormat.Int);
setTag(t, 'Compression', Tiff.Compression.None);
write(t, newData(1:height/2,1:width/4));
close(t);
%}

%{
imwrite(newData(1:width/4,1:height/2), 'Corrected_LDEM_128-0-0.tif', 'tif')
imwrite(newData(1:width/4,(height/2 + 1): height), 'Corrected_LDEM_128-0-1.tif', 'tif')
imwrite(newData((width/4 + 1):width/2,1:height/2), 'Corrected_LDEM_128-1-0.tif', 'tif')
imwrite(newData((width/4 + 1):width/2,1:(height/2 + 1): height), 'Corrected_LDEM_128-1-1.tif', 'tif')
imwrite(newData((width/2 + 1):(width/4 * 3),1:height/2), 'Corrected_LDEM_128-2-0.tif', 'tif')
imwrite(newData((width/2 + 1):(width/4 * 3),1:(height/2 + 1): height), 'Corrected_LDEM_128-2-1.tif', 'tif')
imwrite(newData((width/4 * 3 + 1):width,1:height/2), 'Corrected_LDEM_128-3-0.tif', 'tif')
imwrite(newData((width/4 * 3 + 1):width,1:(height/2 + 1): height), 'Corrected_LDEM_128-3-1.tif', 'tif')
%}

%{
imwrite(rot90(newData(1:height/2,1:width/4), -1), "Corrected_LDEM_128-0-0.tif", "tif", "Compression", "none");
imwrite(newData((height/2 + 1): height,1:width/4), 'Corrected_LDEM_128-0-1.tif', 'tif', "Compression", "none");
imwrite(newData(1:height/2,(width/4 + 1):width/2), 'Corrected_LDEM_128-1-0.tif', 'tif', "Compression", "none");
imwrite(newData((height/2 + 1): height,(width/4 + 1):width/2), 'Corrected_LDEM_128-1-1.tif', 'tif', "Compression", "none");
imwrite(newData(1:height/2,(width/2 + 1):(width/4 * 3)), 'Corrected_LDEM_128-2-0.tif', 'tif', "Compression", "none");
imwrite(newData((height/2 + 1): height,(width/2 + 1):(width/4 * 3)), 'Corrected_LDEM_128-2-1.tif', 'tif', "Compression", "none");
imwrite(newData(1:height/2,(width/4 * 3 + 1):width), 'Corrected_LDEM_128-3-0.tif', 'tif', "Compression", "none");
imwrite(newData((height/2 + 1): height,(width/4 * 3 + -1):width), 'Corrected_LDEM_128-3-1.tif', 'tif', "Compression", "none");
%}
%fclose(fileID);

%A = rand(50);
%disp(A);
%imwrite(A,"myGray.png");
%imwrite(A,"myGray.tif", "tif");

fclose all;