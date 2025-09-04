//
// SPDX-License-Identifier: MIT
//
// This example code file is licensed under the MIT License.
// See https://opensource.org/licenses/MIT
//
// Copyright (c) 2025 LEAP 71 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System.Numerics;
using PicoGK;
using Xunit;

public static class VoxelTests
{
    public static float fSphereVolume(float fRadius)
    {
        return fRadius * fSphereSurface(fRadius) / 3f;
    }

    public static float fSphereSurface(float fRadius)
    {
        return fRadius * fRadius * float.Pi * 4f;
    }

    [Fact]
    public static void CreateNewVoxels()
    {
        using Library lib = new(0.5f);
        Voxels vox = new(lib);
        Assert.True(vox.bIsEmpty());      
    }

    [Fact]
    public static void VoxelSubtractingIsEmpty()
    {
        using Library lib = new(0.5f);
        Voxels vox = Voxels.voxSphere(lib, Vector3.Zero, 15f);
        vox -= vox;
        Assert.True(vox.bIsEmpty());

        vox.CalculateProperties(out float fVolumeCubicMM, out BBox3 oBox);
        
        Assert.Equal(0f, fVolumeCubicMM);
        Assert.True(oBox.bIsEmpty());
        Assert.True(vox.bIsEmpty());
    }

    [Fact]
    public static void VoxelSlices()
    {
        using Library lib = new(0.1f);
        Voxels vox = Voxels.voxSphere(lib, Vector3.Zero, 15f);

        ImageGrayScale img = vox.imgAllocateSlice(out int nSliceCount);
        Assert.Equal(305, img.nWidth);
        Assert.Equal(305, img.nHeight);

        vox.GetVoxelSlice(nSliceCount / 2, ref img);
        vox.GetInterpolatedVoxelSlice(nSliceCount / 2.5f, ref img);
    }

    [Fact]
    public static void VoxelBoundingBoxesAndVolumes()
    {
        {
            using Library lib = new(0.1f);
            Voxels vox = Voxels.voxSphere(lib, Vector3.Zero, 15f);
            Assert.False(vox.bIsEmpty());

            vox.CalculateProperties(out float fVolume, out BBox3 oBBox);

            Assert.True(float.Abs(fSphereVolume(15f) - fVolume) < 1f);

            BBox3 oBBoxComp = new(-15,-15,-15,15,15,15);
            
            Assert.True(float.Abs(oBBox.vecMin.X - oBBoxComp.vecMin.X) < 0.1f);
            Assert.True(float.Abs(oBBox.vecMax.X - oBBoxComp.vecMax.X) < 0.1f);
            Assert.True(oBBox.vecCenter() == Vector3.Zero);
        }
    }
}