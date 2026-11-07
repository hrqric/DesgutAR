import * as THREE from "three";
import { GLTFLoader } from "three/addons/loaders/GLTFLoader.js";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";

let disposeCurrentViewer = null;
let scheduled = false;

function scheduleViewer() {
  if (scheduled) return;
  scheduled = true;
  requestAnimationFrame(() => {
    scheduled = false;
    disposeCurrentViewer?.();
    disposeCurrentViewer = null;
    const canvas = document.querySelector("#dish-model");
    if (canvas) disposeCurrentViewer = createViewer(canvas);
  });
}

function createViewer(canvas) {
  const stage = canvas.closest(".ar-stage");
  const loading = stage.querySelector("#model-loading");
  const scene = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(38, 1, 0.01, 100);
  camera.position.set(2.2, 1.55, 2.45);

  const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: true });
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 1.5));
  renderer.outputColorSpace = THREE.SRGBColorSpace;
  renderer.toneMapping = THREE.ACESFilmicToneMapping;
  renderer.toneMappingExposure = 1.15;
  renderer.shadowMap.enabled = true;
  renderer.shadowMap.type = THREE.PCFShadowMap;

  scene.add(new THREE.HemisphereLight(0xfffbef, 0x53655b, 2.25));
  const key = new THREE.DirectionalLight(0xffffff, 3.2);
  key.position.set(3.5, 5, 3);
  key.castShadow = true;
  key.shadow.mapSize.set(1024, 1024);
  scene.add(key);
  const rim = new THREE.DirectionalLight(0xffd8b8, 1.4);
  rim.position.set(-4, 2, -3);
  scene.add(rim);

  const controls = new OrbitControls(camera, canvas);
  controls.enableDamping = true;
  controls.autoRotate = true;
  controls.autoRotateSpeed = 0.75;
  controls.enablePan = false;
  controls.minDistance = 1.7;
  controls.maxDistance = 4.4;
  controls.minPolarAngle = Math.PI * 0.18;
  controls.maxPolarAngle = Math.PI * 0.48;
  controls.target.set(0, 0, 0);

  const ground = new THREE.Mesh(
    new THREE.CircleGeometry(1.4, 64),
    new THREE.ShadowMaterial({ color: 0x1d2a25, opacity: 0.12 })
  );
  ground.rotation.x = -Math.PI / 2;
  ground.position.y = -0.39;
  ground.receiveShadow = true;
  scene.add(ground);

  let modelRoot = null;
  new GLTFLoader().load(
    "./assets/models/risotto/risotto.gltf",
    (gltf) => {
      modelRoot = gltf.scene;
      const box = new THREE.Box3().setFromObject(modelRoot);
      const size = box.getSize(new THREE.Vector3());
      const center = box.getCenter(new THREE.Vector3());
      const scale = 2 / Math.max(size.x, size.y, size.z);
      modelRoot.scale.setScalar(scale);
      modelRoot.position.set(-center.x * scale, -center.y * scale - 0.18, -center.z * scale);
      modelRoot.traverse((node) => {
        if (!node.isMesh) return;
        node.castShadow = true;
        node.receiveShadow = true;
        if (node.material?.map) node.material.map.colorSpace = THREE.SRGBColorSpace;
      });
      scene.add(modelRoot);
      loading?.classList.add("loaded");
    },
    undefined,
    (error) => {
      console.error("Não foi possível carregar o modelo 3D:", error);
      if (loading) {
        loading.textContent = "Não foi possível carregar o modelo 3D";
        loading.classList.add("error");
      }
    }
  );

  const resize = () => {
    const width = Math.max(stage.clientWidth, 1);
    const height = Math.max(stage.clientHeight, 1);
    camera.aspect = width / height;
    camera.updateProjectionMatrix();
    renderer.setSize(width, height, false);
  };
  const resizeObserver = new ResizeObserver(resize);
  resizeObserver.observe(stage);
  resize();

  renderer.setAnimationLoop(() => {
    controls.update();
    renderer.render(scene, camera);
  });

  return () => {
    resizeObserver.disconnect();
    renderer.setAnimationLoop(null);
    controls.dispose();
    scene.traverse((node) => {
      node.geometry?.dispose?.();
      const materials = Array.isArray(node.material) ? node.material : [node.material];
      for (const material of materials) {
        if (!material) continue;
        for (const value of Object.values(material)) {
          if (value?.isTexture) value.dispose();
        }
        material.dispose?.();
      }
    });
    renderer.dispose();
  };
}

new MutationObserver(scheduleViewer).observe(document.querySelector("#view"), { childList: true, subtree: true });
scheduleViewer();
