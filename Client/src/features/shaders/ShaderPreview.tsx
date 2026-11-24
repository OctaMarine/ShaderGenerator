
import { useEffect, useMemo } from "react";
import { Canvas } from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";

import * as THREE from "three";
import type { GeometryType } from "../../types/shader";

interface ShaderPreviewProps {
    geometryType: GeometryType;
    vertexShader: string;
    fragmentShader: string;
}

function createGeometry(type: GeometryType): THREE.BufferGeometry {
    switch (type) {
        case "octahedron":
            return new THREE.OctahedronGeometry(1.2, 1);

        case "icosahedron": {
            const geometry = new THREE.IcosahedronGeometry(1.2, 1).toNonIndexed();
            geometry.computeVertexNormals();
            return geometry;
        }

        case "sphere":
            return new THREE.SphereGeometry(1, 48, 48);

        default:
            throw new Error(`Unsupported geometry type: ${type}`);
    }
}

function normalizeShader(source: string): string {
    return source
        .replace(/^\uFEFF?\s*#version\s+300\s+es\s*\r?\n?/, "")
        .trim();
}

function ShaderMesh({
                        geometryType,
                        vertexShader,
                        fragmentShader
                    }: ShaderPreviewProps) {
    
    const geometry = useMemo(
        () => createGeometry(geometryType),
        [geometryType]
    );
    
    useEffect(() => {
        return () => {
            geometry.dispose();
        };
    }, [geometry]);
    
    const normalizedVertexShader = useMemo(
        () => normalizeShader(vertexShader),
        [vertexShader]
    );

    const normalizedFragmentShader = useMemo(
        () => normalizeShader(fragmentShader),
        [fragmentShader]
    );

    return (
        <mesh geometry={geometry}>
            <shaderMaterial
                vertexShader={normalizedVertexShader}
                fragmentShader={normalizedFragmentShader}
            />
        </mesh>
    );
}

export function ShaderPreview({
                                  geometryType,
                                  vertexShader,
                                  fragmentShader
                              }: ShaderPreviewProps) {

    return (
        <div className="shader-preview">
            <Canvas
                camera={{
                    position: [0, 0, 5],
                    fov: 45
                }}
                dpr={[1, 2]}
            >
                <color
                    attach="background"
                    args={["#080d18"]}
                />

                <ShaderMesh
                    geometryType={geometryType}
                    vertexShader={vertexShader}
                    fragmentShader={fragmentShader}
                />

                <OrbitControls
                    makeDefault
                    enableRotate
                    enableZoom
                    enablePan={false}
                    minDistance={3}
                    maxDistance={8}
                />
            </Canvas>
        </div>
    );
}
