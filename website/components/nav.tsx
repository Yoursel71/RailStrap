import Image from "next/image";
import Link from "next/link";
import { Github, Download } from "lucide-react";
import { Button } from "@/components/ui/button";
import { siteAsset } from "@/lib/site";

export function Nav() {
  return (
    <header className="sticky top-0 z-50 border-b border-white/[0.07] bg-[#09090f]/80 backdrop-blur-xl">
      <nav className="container flex h-16 items-center justify-between" aria-label="Primary navigation">
        <Link href="#hero" className="flex items-center gap-2 text-[15px] font-bold tracking-tight">
          <Image src={siteAsset("/logo.png")} alt="RailStrap logo" width={24} height={24} className="rounded-md" />
          RailStrap
        </Link>

        <div className="flex items-center gap-2.5">
          <Button variant="ghost" size="sm" className="hidden sm:inline-flex" asChild>
            <a href="https://github.com/Yoursel71/RailStrap" target="_blank" rel="noopener noreferrer">
              <Github /> GitHub
            </a>
          </Button>
          <Button size="sm" asChild>
            <a href="https://github.com/Yoursel71/RailStrap/releases/latest" target="_blank" rel="noopener noreferrer">
              <Download /> Download
            </a>
          </Button>
        </div>
      </nav>
    </header>
  );
}
